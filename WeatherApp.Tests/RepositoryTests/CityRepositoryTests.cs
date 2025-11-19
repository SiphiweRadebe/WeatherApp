using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data.Context;
using WeatherApp.Data.Entities;
using WeatherApp.Data.Repositories;

namespace WeatherApp.Tests.RepositoryTests;

[TestFixture]
public class CityRepositoryTests
{
    private WeatherDbContext _context;
    private CityRepository _cityRepository;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<WeatherDbContext>()
            .UseInMemoryDatabase(databaseName: $"CityTestDb_{Guid.NewGuid()}")
            .Options;

        _context = new WeatherDbContext(options);
        _cityRepository = new CityRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetByIdAsync_WhenCityExists_ReturnsCity()
    {
        // Arrange
        var city = new City
        {
            Name = "New York",
            Country = "USA",
            Latitude = 40.7128m,
            Longitude = -74.0060m,
            TimeZone = "America/New_York",
            CreatedAt = DateTime.UtcNow
        };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        // Act
        var result = await _cityRepository.GetByIdAsync(city.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("New York");
        result.Country.Should().Be("USA");
    }

    [Test]
    public async Task GetByIdAsync_WhenCityDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _cityRepository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllCities()
    {
        // Arrange
        var cities = new[]
        {
            new City { Name = "New York", Country = "USA", Latitude = 40.7128m, Longitude = -74.0060m, CreatedAt = DateTime.UtcNow },
            new City { Name = "London", Country = "UK", Latitude = 51.5074m, Longitude = -0.1278m, CreatedAt = DateTime.UtcNow },
            new City { Name = "Tokyo", Country = "Japan", Latitude = 35.6762m, Longitude = 139.6503m, CreatedAt = DateTime.UtcNow }
        };
        _context.Cities.AddRange(cities);
        await _context.SaveChangesAsync();

        // Act
        var result = await _cityRepository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.Name == "New York");
        result.Should().Contain(c => c.Name == "London");
        result.Should().Contain(c => c.Name == "Tokyo");
    }

    [Test]
    public async Task GetByNameAndCountryAsync_WhenCityExists_ReturnsCity()
    {
        // Arrange
        var city = new City
        {
            Name = "Paris",
            Country = "France",
            Latitude = 48.8566m,
            Longitude = 2.3522m,
            CreatedAt = DateTime.UtcNow
        };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        // Act
        var result = await _cityRepository.GetByNameAndCountryAsync("Paris", "France");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Paris");
        result.Country.Should().Be("France");
    }

    [Test]
    public async Task GetByNameAndCountryAsync_WhenCityDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _cityRepository.GetByNameAndCountryAsync("NonExistent", "Country");

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetCitiesWithWeatherRecordsAsync_ReturnsAllCitiesWithWeatherRecords()
    {
        // Arrange
        var city1 = new City
        {
            Name = "New York",
            Country = "USA",
            Latitude = 40.7128m,
            Longitude = -74.0060m,
            CreatedAt = DateTime.UtcNow
        };
        var city2 = new City
        {
            Name = "London",
            Country = "UK",
            Latitude = 51.5074m,
            Longitude = -0.1278m,
            CreatedAt = DateTime.UtcNow
        };
        _context.Cities.AddRange(city1, city2);
        await _context.SaveChangesAsync();

        var weatherRecord1 = new WeatherRecord
        {
            CityId = city1.Id,
            Temperature = 25.5m,
            Humidity = 60,
            ObservationTime = DateTime.UtcNow,
            Condition = "Clear",
            CreatedAt = DateTime.UtcNow
        };
        var weatherRecord2 = new WeatherRecord
        {
            CityId = city1.Id,
            Temperature = 26.0m,
            Humidity = 65,
            ObservationTime = DateTime.UtcNow.AddHours(1),
            Condition = "Cloudy",
            CreatedAt = DateTime.UtcNow
        };
        _context.WeatherRecords.AddRange(weatherRecord1, weatherRecord2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _cityRepository.GetCitiesWithWeatherRecordsAsync();

        // Assert
        result.Should().HaveCount(2);
        var cityWithRecords = result.First(c => c.Name == "New York");
        cityWithRecords.WeatherRecords.Should().HaveCount(2);
    }

    [Test]
    public async Task GetCityWithAlertsAsync_ReturnsCityWithAlerts()
    {
        // Arrange
        var city = new City
        {
            Name = "New York",
            Country = "USA",
            Latitude = 40.7128m,
            Longitude = -74.0060m,
            CreatedAt = DateTime.UtcNow
        };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var alert = new Alert
        {
            Title = "Heat Wave",
            Description = "Extreme temperatures",
            Severity = "High",
            AlertType = "Temperature",
            StartTime = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Alerts.Add(alert);
        await _context.SaveChangesAsync();

        var cityAlert = new CityAlert
        {
            CityId = city.Id,
            AlertId = alert.Id,
            AssociatedAt = DateTime.UtcNow,
            NotificationSent = false
        };
        _context.CityAlerts.Add(cityAlert);
        await _context.SaveChangesAsync();

        // Act
        var result = await _cityRepository.GetCityWithAlertsAsync(city.Id);

        // Assert
        result.Should().NotBeNull();
        result!.CityAlerts.Should().HaveCount(1);
        result.CityAlerts.First().Alert.Title.Should().Be("Heat Wave");
    }

    [Test]
    public async Task AddAsync_AddsCity()
    {
        // Arrange
        var city = new City
        {
            Name = "Berlin",
            Country = "Germany",
            Latitude = 52.5200m,
            Longitude = 13.4050m,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _cityRepository.AddAsync(city);

        // Assert
        var result = await _context.Cities.FindAsync(city.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be("Berlin");
    }

    [Test]
    public async Task UpdateAsync_UpdatesCity()
    {
        // Arrange
        var city = new City
        {
            Name = "Madrid",
            Country = "Spain",
            Latitude = 40.4168m,
            Longitude = -3.7038m,
            CreatedAt = DateTime.UtcNow
        };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        // Act
        city.Name = "Madrid City";
        city.TimeZone = "Europe/Madrid";
        city.UpdatedAt = DateTime.UtcNow;
        await _cityRepository.UpdateAsync(city);

        // Assert
        var result = await _context.Cities.FindAsync(city.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be("Madrid City");
        result.TimeZone.Should().Be("Europe/Madrid");
        result.UpdatedAt.Should().NotBeNull();
    }

    [Test]
    public async Task DeleteAsync_DeletesCity()
    {
        // Arrange
        var city = new City
        {
            Name = "Rome",
            Country = "Italy",
            Latitude = 41.9028m,
            Longitude = 12.4964m,
            CreatedAt = DateTime.UtcNow
        };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        // Act
        var result = await _cityRepository.DeleteAsync(city.Id);

        // Assert
        result.Should().BeTrue();
        var deletedCity = await _context.Cities.FindAsync(city.Id);
        deletedCity.Should().BeNull();
    }

    [Test]
    public async Task DeleteAsync_WhenCityDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _cityRepository.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }
}
