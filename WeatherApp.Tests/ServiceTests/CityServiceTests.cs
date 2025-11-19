using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherApp.Core.DTOs;
using WeatherApp.Core.Exceptions;
using WeatherApp.Core.Services;
using WeatherApp.Data.Entities;
using WeatherApp.Data.Repositories;

namespace WeatherApp.Tests.ServiceTests;

[TestFixture]
public class CityServiceTests
{
    private Mock<ICityRepository> _mockCityRepository;
    private Mock<ILogger<CityService>> _mockLogger;
    private CityService _cityService;

    [SetUp]
    public void SetUp()
    {
        _mockCityRepository = new Mock<ICityRepository>();
        _mockLogger = new Mock<ILogger<CityService>>();
        _cityService = new CityService(_mockCityRepository.Object, _mockLogger.Object);
    }

    [Test]
    public async Task GetByIdAsync_WhenCityExists_ReturnsCityDto()
    {
        // Arrange
        var cityId = 1;
        var city = new City
        {
            Id = cityId,
            Name = "New York",
            Country = "USA",
            Latitude = 40.7128m,
            Longitude = -74.0060m,
            TimeZone = "America/New_York",
            CreatedAt = DateTime.UtcNow
        };
        _mockCityRepository.Setup(r => r.GetByIdAsync(cityId))
            .ReturnsAsync(city);

        // Act
        var result = await _cityService.GetByIdAsync(cityId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cityId);
        result.Name.Should().Be("New York");
        result.Country.Should().Be("USA");
        result.Latitude.Should().Be(40.7128m);
        result.Longitude.Should().Be(-74.0060m);
    }

    [Test]
    public void GetByIdAsync_WhenCityDoesNotExist_ThrowsEntityNotFoundException()
    {
        // Arrange
        var cityId = 999;
        _mockCityRepository.Setup(r => r.GetByIdAsync(cityId))
            .ReturnsAsync((City?)null);

        // Act & Assert
        Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _cityService.GetByIdAsync(cityId));
    }

    [Test]
    public async Task GetAllAsync_ReturnsCityDtoList()
    {
        // Arrange
        var cities = new List<City>
        {
            new City { Id = 1, Name = "New York", Country = "USA", Latitude = 40.7128m, Longitude = -74.0060m, CreatedAt = DateTime.UtcNow },
            new City { Id = 2, Name = "London", Country = "UK", Latitude = 51.5074m, Longitude = -0.1278m, CreatedAt = DateTime.UtcNow }
        };
        _mockCityRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(cities);

        // Act
        var result = await _cityService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "New York");
        result.Should().Contain(c => c.Name == "London");
    }

    [Test]
    public async Task CreateAsync_WithValidData_CreatesCity()
    {
        // Arrange
        var createDto = new CreateCityDto
        {
            Name = "Tokyo",
            Country = "Japan",
            Latitude = 35.6762m,
            Longitude = 139.6503m,
            TimeZone = "Asia/Tokyo"
        };

        _mockCityRepository.Setup(r => r.GetByNameAndCountryAsync(createDto.Name, createDto.Country))
            .ReturnsAsync((City?)null);
        _mockCityRepository.Setup(r => r.AddAsync(It.IsAny<City>()))
            .ReturnsAsync((City c) => c);

        // Act
        var result = await _cityService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Tokyo");
        result.Country.Should().Be("Japan");
        result.Latitude.Should().Be(35.6762m);
        result.Longitude.Should().Be(139.6503m);
        _mockCityRepository.Verify(r => r.AddAsync(It.IsAny<City>()), Times.Once);
    }

    [Test]
    public void CreateAsync_WhenCityAlreadyExists_ThrowsDuplicateEntityException()
    {
        // Arrange
        var createDto = new CreateCityDto
        {
            Name = "Tokyo",
            Country = "Japan",
            Latitude = 35.6762m,
            Longitude = 139.6503m
        };

        var existingCity = new City
        {
            Id = 1,
            Name = "Tokyo",
            Country = "Japan",
            Latitude = 35.6762m,
            Longitude = 139.6503m,
            CreatedAt = DateTime.UtcNow
        };

        _mockCityRepository.Setup(r => r.GetByNameAndCountryAsync(createDto.Name, createDto.Country))
            .ReturnsAsync(existingCity);

        // Act & Assert
        Assert.ThrowsAsync<DuplicateEntityException>(async () =>
            await _cityService.CreateAsync(createDto));
    }

    [Test]
    public void CreateAsync_WithInvalidLatitude_ThrowsBusinessException()
    {
        // Arrange
        var createDto = new CreateCityDto
        {
            Name = "Invalid City",
            Country = "Test",
            Latitude = 100m, // Invalid: > 90
            Longitude = 0m
        };

        _mockCityRepository.Setup(r => r.GetByNameAndCountryAsync(createDto.Name, createDto.Country))
            .ReturnsAsync((City?)null);

        // Act & Assert
        Assert.ThrowsAsync<BusinessException>(async () =>
            await _cityService.CreateAsync(createDto));
    }

    [Test]
    public void CreateAsync_WithInvalidLongitude_ThrowsBusinessException()
    {
        // Arrange
        var createDto = new CreateCityDto
        {
            Name = "Invalid City",
            Country = "Test",
            Latitude = 0m,
            Longitude = 200m // Invalid: > 180
        };

        _mockCityRepository.Setup(r => r.GetByNameAndCountryAsync(createDto.Name, createDto.Country))
            .ReturnsAsync((City?)null);

        // Act & Assert
        Assert.ThrowsAsync<BusinessException>(async () =>
            await _cityService.CreateAsync(createDto));
    }

    [Test]
    public async Task UpdateAsync_WithValidData_UpdatesCity()
    {
        // Arrange
        var cityId = 1;
        var existingCity = new City
        {
            Id = cityId,
            Name = "New York",
            Country = "USA",
            Latitude = 40.7128m,
            Longitude = -74.0060m,
            CreatedAt = DateTime.UtcNow
        };

        var updateDto = new UpdateCityDto
        {
            Name = "New York City",
            TimeZone = "America/New_York"
        };

        _mockCityRepository.Setup(r => r.GetByIdAsync(cityId))
            .ReturnsAsync(existingCity);
        _mockCityRepository.Setup(r => r.UpdateAsync(It.IsAny<City>()))
            .ReturnsAsync((City c) => c);

        // Act
        var result = await _cityService.UpdateAsync(cityId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New York City");
        result.TimeZone.Should().Be("America/New_York");
        _mockCityRepository.Verify(r => r.UpdateAsync(It.IsAny<City>()), Times.Once);
    }

    [Test]
    public void UpdateAsync_WhenCityDoesNotExist_ThrowsEntityNotFoundException()
    {
        // Arrange
        var cityId = 999;
        var updateDto = new UpdateCityDto { Name = "Test City" };

        _mockCityRepository.Setup(r => r.GetByIdAsync(cityId))
            .ReturnsAsync((City?)null);

        // Act & Assert
        Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _cityService.UpdateAsync(cityId, updateDto));
    }

    [Test]
    public async Task DeleteAsync_WhenCityExists_ReturnsTrue()
    {
        // Arrange
        var cityId = 1;
        _mockCityRepository.Setup(r => r.DeleteAsync(cityId))
            .ReturnsAsync(true);

        // Act
        var result = await _cityService.DeleteAsync(cityId);

        // Assert
        result.Should().BeTrue();
        _mockCityRepository.Verify(r => r.DeleteAsync(cityId), Times.Once);
    }

    [Test]
    public async Task GetCityWithWeatherAsync_WhenCityExists_ReturnsCityWithWeatherRecords()
    {
        // Arrange
        var cityId = 1;
        var city = new City
        {
            Id = cityId,
            Name = "New York",
            Country = "USA",
            Latitude = 40.7128m,
            Longitude = -74.0060m,
            CreatedAt = DateTime.UtcNow,
            WeatherRecords = new List<WeatherRecord>
            {
                new WeatherRecord
                {
                    Id = 1,
                    CityId = cityId,
                    Temperature = 25.5m,
                    Humidity = 60,
                    ObservationTime = DateTime.UtcNow,
                    Condition = "Clear",
                    CreatedAt = DateTime.UtcNow
                }
            }
        };

        var cities = new List<City> { city };
        _mockCityRepository.Setup(r => r.GetCitiesWithWeatherRecordsAsync())
            .ReturnsAsync(cities);

        // Act
        var result = await _cityService.GetCityWithWeatherAsync(cityId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cityId);
        result.WeatherRecords.Should().HaveCount(1);
        result.WeatherRecords.First().Temperature.Should().Be(25.5m);
    }
}
