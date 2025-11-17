using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data.Context;
using WeatherApp.Data.Entities;
using WeatherApp.Data.Repositories;

namespace WeatherApp.Tests.Repositories;

[TestFixture]
public class WeatherRecordRepositoryTests
{
    private WeatherDbContext _context;
    private WeatherRecordRepository _weatherRecordRepository;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<WeatherDbContext>()
            .UseInMemoryDatabase(databaseName: $"WeatherRecordTestDb_{Guid.NewGuid()}")
            .Options;

        _context = new WeatherDbContext(options);
        _weatherRecordRepository = new WeatherRecordRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetByIdAsync_WhenRecordExists_ReturnsWeatherRecord()
    {
        // Arrange
        var city = new City { Name = "New York", Country = "USA", Latitude = 40.7128m, Longitude = -74.0060m, CreatedAt = DateTime.UtcNow };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var weatherRecord = new WeatherRecord
        {
            CityId = city.Id,
            Temperature = 25.5m,
            FeelsLike = 24.0m,
            Humidity = 60,
            WindSpeed = 10.5m,
            WindDirection = "NE",
            Pressure = 1013,
            Condition = "Clear",
            Description = "Clear sky",
            ObservationTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        _context.WeatherRecords.Add(weatherRecord);
        await _context.SaveChangesAsync();

        // Act
        var result = await _weatherRecordRepository.GetByIdAsync(weatherRecord.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Temperature.Should().Be(25.5m);
        result.Humidity.Should().Be(60);
    }

    [Test]
    public async Task GetByCityIdAsync_ReturnsRecordsForCityOrderedByObservationTime()
    {
        // Arrange
        var city = new City { Name = "London", Country = "UK", Latitude = 51.5074m, Longitude = -0.1278m, CreatedAt = DateTime.UtcNow };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var baseTime = DateTime.UtcNow;
        var records = new[]
        {
            new WeatherRecord { CityId = city.Id, Temperature = 20m, Humidity = 50, ObservationTime = baseTime.AddHours(-2), Condition = "Clear", CreatedAt = DateTime.UtcNow },
            new WeatherRecord { CityId = city.Id, Temperature = 22m, Humidity = 55, ObservationTime = baseTime, Condition = "Cloudy", CreatedAt = DateTime.UtcNow },
            new WeatherRecord { CityId = city.Id, Temperature = 21m, Humidity = 52, ObservationTime = baseTime.AddHours(-1), Condition = "Partly Cloudy", CreatedAt = DateTime.UtcNow }
        };
        _context.WeatherRecords.AddRange(records);
        await _context.SaveChangesAsync();

        // Act
        var result = await _weatherRecordRepository.GetByCityIdAsync(city.Id);

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeInDescendingOrder(r => r.ObservationTime);
        result.First().Temperature.Should().Be(22m); // Most recent
    }

    [Test]
    public async Task GetRecentByCityIdAsync_ReturnsLimitedNumberOfRecords()
    {
        // Arrange
        var city = new City { Name = "Tokyo", Country = "Japan", Latitude = 35.6762m, Longitude = 139.6503m, CreatedAt = DateTime.UtcNow };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var records = new List<WeatherRecord>();
        for (int i = 0; i < 15; i++)
        {
            records.Add(new WeatherRecord
            {
                CityId = city.Id,
                Temperature = 20m + i,
                Humidity = 50,
                ObservationTime = DateTime.UtcNow.AddHours(-i),
                Condition = "Clear",
                CreatedAt = DateTime.UtcNow
            });
        }
        _context.WeatherRecords.AddRange(records);
        await _context.SaveChangesAsync();

        // Act
        var result = await _weatherRecordRepository.GetRecentByCityIdAsync(city.Id, 5);

        // Assert
        result.Should().HaveCount(5);
        result.Should().BeInDescendingOrder(r => r.ObservationTime);
    }

    [Test]
    public async Task GetLatestByCityIdAsync_ReturnsNewestRecord()
    {
        // Arrange
        var city = new City { Name = "Paris", Country = "France", Latitude = 48.8566m, Longitude = 2.3522m, CreatedAt = DateTime.UtcNow };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var baseTime = DateTime.UtcNow;
        var records = new[]
        {
            new WeatherRecord { CityId = city.Id, Temperature = 18m, Humidity = 55, ObservationTime = baseTime.AddHours(-2), Condition = "Rainy", CreatedAt = DateTime.UtcNow },
            new WeatherRecord { CityId = city.Id, Temperature = 20m, Humidity = 60, ObservationTime = baseTime.AddHours(-1), Condition = "Cloudy", CreatedAt = DateTime.UtcNow },
            new WeatherRecord { CityId = city.Id, Temperature = 22m, Humidity = 58, ObservationTime = baseTime, Condition = "Clear", CreatedAt = DateTime.UtcNow }
        };
        _context.WeatherRecords.AddRange(records);
        await _context.SaveChangesAsync();

        // Act
        var result = await _weatherRecordRepository.GetLatestByCityIdAsync(city.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Temperature.Should().Be(22m);
        result.ObservationTime.Should().Be(baseTime);
    }

    [Test]
    public async Task GetLatestByCityIdAsync_WhenNoRecords_ReturnsNull()
    {
        // Arrange
        var city = new City { Name = "Berlin", Country = "Germany", Latitude = 52.5200m, Longitude = 13.4050m, CreatedAt = DateTime.UtcNow };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        // Act
        var result = await _weatherRecordRepository.GetLatestByCityIdAsync(city.Id);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetByDateRangeAsync_ReturnsRecordsInRange()
    {
        // Arrange
        var city = new City { Name = "Madrid", Country = "Spain", Latitude = 40.4168m, Longitude = -3.7038m, CreatedAt = DateTime.UtcNow };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var baseTime = DateTime.UtcNow;
        var records = new[]
        {
            new WeatherRecord { CityId = city.Id, Temperature = 18m, Humidity = 50, ObservationTime = baseTime.AddDays(-3), Condition = "Clear", CreatedAt = DateTime.UtcNow },
            new WeatherRecord { CityId = city.Id, Temperature = 20m, Humidity = 55, ObservationTime = baseTime.AddDays(-2), Condition = "Cloudy", CreatedAt = DateTime.UtcNow },
            new WeatherRecord { CityId = city.Id, Temperature = 22m, Humidity = 60, ObservationTime = baseTime.AddDays(-1), Condition = "Sunny", CreatedAt = DateTime.UtcNow },
            new WeatherRecord { CityId = city.Id, Temperature = 24m, Humidity = 58, ObservationTime = baseTime, Condition = "Clear", CreatedAt = DateTime.UtcNow },
            new WeatherRecord { CityId = city.Id, Temperature = 26m, Humidity = 52, ObservationTime = baseTime.AddDays(1), Condition = "Sunny", CreatedAt = DateTime.UtcNow }
        };
        _context.WeatherRecords.AddRange(records);
        await _context.SaveChangesAsync();

        // Act
        var startDate = baseTime.AddDays(-2);
        var endDate = baseTime;
        var result = await _weatherRecordRepository.GetByDateRangeAsync(city.Id, startDate, endDate);

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeInAscendingOrder(r => r.ObservationTime);
        result.First().Temperature.Should().Be(20m);
        result.Last().Temperature.Should().Be(24m);
    }

    [Test]
    public async Task AddAsync_AddsWeatherRecord()
    {
        // Arrange
        var city = new City { Name = "Rome", Country = "Italy", Latitude = 41.9028m, Longitude = 12.4964m, CreatedAt = DateTime.UtcNow };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var weatherRecord = new WeatherRecord
        {
            CityId = city.Id,
            Temperature = 28.5m,
            FeelsLike = 27.0m,
            Humidity = 65,
            WindSpeed = 8.5m,
            Pressure = 1015,
            Condition = "Sunny",
            Description = "Bright and sunny",
            ObservationTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _weatherRecordRepository.AddAsync(weatherRecord);

        // Assert
        var result = await _context.WeatherRecords.FindAsync(weatherRecord.Id);
        result.Should().NotBeNull();
        result!.Temperature.Should().Be(28.5m);
        result.Condition.Should().Be("Sunny");
    }

    [Test]
    public async Task DeleteAsync_DeletesWeatherRecord()
    {
        // Arrange
        var city = new City { Name = "Amsterdam", Country = "Netherlands", Latitude = 52.3676m, Longitude = 4.9041m, CreatedAt = DateTime.UtcNow };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var weatherRecord = new WeatherRecord
        {
            CityId = city.Id,
            Temperature = 15m,
            Humidity = 70,
            ObservationTime = DateTime.UtcNow,
            Condition = "Rainy",
            CreatedAt = DateTime.UtcNow
        };
        _context.WeatherRecords.Add(weatherRecord);
        await _context.SaveChangesAsync();

        // Act
        var result = await _weatherRecordRepository.DeleteAsync(weatherRecord.Id);

        // Assert
        result.Should().BeTrue();
        var deletedRecord = await _context.WeatherRecords.FindAsync(weatherRecord.Id);
        deletedRecord.Should().BeNull();
    }

    [Test]
    public async Task GetByCityIdAsync_IncludesCityInformation()
    {
        // Arrange
        var city = new City { Name = "Barcelona", Country = "Spain", Latitude = 41.3851m, Longitude = 2.1734m, CreatedAt = DateTime.UtcNow };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var weatherRecord = new WeatherRecord
        {
            CityId = city.Id,
            Temperature = 26m,
            Humidity = 55,
            ObservationTime = DateTime.UtcNow,
            Condition = "Clear",
            CreatedAt = DateTime.UtcNow
        };
        _context.WeatherRecords.Add(weatherRecord);
        await _context.SaveChangesAsync();

        // Act
        var result = await _weatherRecordRepository.GetByCityIdAsync(city.Id);

        // Assert
        result.Should().HaveCount(1);
        var record = result.First();
        record.City.Should().NotBeNull();
        record.City!.Name.Should().Be("Barcelona");
    }
}
