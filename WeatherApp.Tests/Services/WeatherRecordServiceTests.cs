using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherApp.Core.DTOs;
using WeatherApp.Core.Exceptions;
using WeatherApp.Core.IService;
using WeatherApp.Core.Services;
using WeatherApp.Data.Entities;
using WeatherApp.Data.Repositories;

namespace WeatherApp.Tests.Services;

[TestFixture]
public class WeatherRecordServiceTests
{
    private Mock<IWeatherRecordRepository> _mockWeatherRecordRepository;
    private Mock<ICityRepository> _mockCityRepository;
    private Mock<IAlertService> _mockAlertService;
    private Mock<ILogger<WeatherRecordService>> _mockLogger;
    private WeatherRecordService _weatherRecordService;

    [SetUp]
    public void SetUp()
    {
        _mockWeatherRecordRepository = new Mock<IWeatherRecordRepository>();
        _mockCityRepository = new Mock<ICityRepository>();
        _mockAlertService = new Mock<IAlertService>();
        _mockLogger = new Mock<ILogger<WeatherRecordService>>();
        _weatherRecordService = new WeatherRecordService(
            _mockWeatherRecordRepository.Object,
            _mockCityRepository.Object,
            _mockAlertService.Object,
            _mockLogger.Object);
    }

    [Test]
    public async Task GetByIdAsync_WhenRecordExists_ReturnsWeatherRecordDto()
    {
        // Arrange
        var recordId = 1;
        var record = new WeatherRecord
        {
            Id = recordId,
            CityId = 1,
            Temperature = 25.5m,
            FeelsLike = 24.0m,
            Humidity = 60,
            WindSpeed = 10.5m,
            Pressure = 1013,
            Condition = "Clear",
            Description = "Clear sky",
            ObservationTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _mockWeatherRecordRepository.Setup(r => r.GetByIdAsync(recordId))
            .ReturnsAsync(record);

        // Act
        var result = await _weatherRecordService.GetByIdAsync(recordId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(recordId);
        result.Temperature.Should().Be(25.5m);
        result.Humidity.Should().Be(60);
    }

    [Test]
    public void GetByIdAsync_WhenRecordDoesNotExist_ThrowsEntityNotFoundException()
    {
        // Arrange
        var recordId = 999;
        _mockWeatherRecordRepository.Setup(r => r.GetByIdAsync(recordId))
            .ReturnsAsync((WeatherRecord?)null);

        // Act & Assert
        Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _weatherRecordService.GetByIdAsync(recordId));
    }

    [Test]
    public async Task GetByCityIdAsync_ReturnsRecordsForCity()
    {
        // Arrange
        var cityId = 1;
        var records = new List<WeatherRecord>
        {
            new WeatherRecord { Id = 1, CityId = cityId, Temperature = 20, Humidity = 50, ObservationTime = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, Condition = "Clear" },
            new WeatherRecord { Id = 2, CityId = cityId, Temperature = 22, Humidity = 55, ObservationTime = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, Condition = "Cloudy" }
        };

        _mockWeatherRecordRepository.Setup(r => r.GetByCityIdAsync(cityId))
            .ReturnsAsync(records);

        // Act
        var result = await _weatherRecordService.GetByCityIdAsync(cityId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(r => r.CityId == cityId);
    }

    [Test]
    public async Task GetLatestByCityIdAsync_ReturnsLatestRecord()
    {
        // Arrange
        var cityId = 1;
        var record = new WeatherRecord
        {
            Id = 1,
            CityId = cityId,
            Temperature = 25,
            Humidity = 60,
            ObservationTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Condition = "Clear"
        };

        _mockWeatherRecordRepository.Setup(r => r.GetLatestByCityIdAsync(cityId))
            .ReturnsAsync(record);

        // Act
        var result = await _weatherRecordService.GetLatestByCityIdAsync(cityId);

        // Assert
        result.Should().NotBeNull();
        result!.CityId.Should().Be(cityId);
    }

    [Test]
    public async Task CreateAsync_WithValidData_CreatesWeatherRecord()
    {
        // Arrange
        var cityId = 1;
        var createDto = new CreateWeatherRecordDto
        {
            CityId = cityId,
            Temperature = 22.5m,
            FeelsLike = 21.0m,
            Humidity = 65,
            WindSpeed = 15.0m,
            Pressure = 1015,
            Condition = "Cloudy",
            Description = "Partly cloudy"
        };

        var city = new City { Id = cityId, Name = "Test City", Country = "Test", Latitude = 0, Longitude = 0, CreatedAt = DateTime.UtcNow };

        _mockCityRepository.Setup(r => r.GetByIdAsync(cityId))
            .ReturnsAsync(city);
        _mockWeatherRecordRepository.Setup(r => r.AddAsync(It.IsAny<WeatherRecord>()))
            .ReturnsAsync((WeatherRecord w) => w);

        // Act
        var result = await _weatherRecordService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.CityId.Should().Be(cityId);
        result.Temperature.Should().Be(22.5m);
        result.Humidity.Should().Be(65);
        _mockWeatherRecordRepository.Verify(r => r.AddAsync(It.IsAny<WeatherRecord>()), Times.Once);
    }

    [Test]
    public void CreateAsync_WhenCityDoesNotExist_ThrowsEntityNotFoundException()
    {
        // Arrange
        var createDto = new CreateWeatherRecordDto
        {
            CityId = 999,
            Temperature = 20,
            Humidity = 50
        };

        _mockCityRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((City?)null);

        // Act & Assert
        Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _weatherRecordService.CreateAsync(createDto));
    }

    [Test]
    public void CreateAsync_WithInvalidTemperature_ThrowsBusinessException()
    {
        // Arrange
        var cityId = 1;
        var createDto = new CreateWeatherRecordDto
        {
            CityId = cityId,
            Temperature = -150m, // Invalid: < -100
            Humidity = 50
        };

        var city = new City { Id = cityId, Name = "Test", Country = "Test", Latitude = 0, Longitude = 0, CreatedAt = DateTime.UtcNow };
        _mockCityRepository.Setup(r => r.GetByIdAsync(cityId))
            .ReturnsAsync(city);

        // Act & Assert
        Assert.ThrowsAsync<BusinessException>(async () =>
            await _weatherRecordService.CreateAsync(createDto));
    }

    [Test]
    public void CreateAsync_WithInvalidHumidity_ThrowsBusinessException()
    {
        // Arrange
        var cityId = 1;
        var createDto = new CreateWeatherRecordDto
        {
            CityId = cityId,
            Temperature = 20,
            Humidity = 150 // Invalid: > 100
        };

        var city = new City { Id = cityId, Name = "Test", Country = "Test", Latitude = 0, Longitude = 0, CreatedAt = DateTime.UtcNow };
        _mockCityRepository.Setup(r => r.GetByIdAsync(cityId))
            .ReturnsAsync(city);

        // Act & Assert
        Assert.ThrowsAsync<BusinessException>(async () =>
            await _weatherRecordService.CreateAsync(createDto));
    }

    [Test]
    public async Task CreateAsync_WithExtremeHeat_CreatesAlert()
    {
        // Arrange
        var cityId = 1;
        var createDto = new CreateWeatherRecordDto
        {
            CityId = cityId,
            Temperature = 40m, // Extreme heat
            Humidity = 50
        };

        var city = new City { Id = cityId, Name = "Test City", Country = "Test", Latitude = 0, Longitude = 0, CreatedAt = DateTime.UtcNow };

        _mockCityRepository.Setup(r => r.GetByIdAsync(cityId))
            .ReturnsAsync(city);
        _mockWeatherRecordRepository.Setup(r => r.AddAsync(It.IsAny<WeatherRecord>()))
            .ReturnsAsync((WeatherRecord w) => w);
        _mockAlertService.Setup(s => s.CreateAsync(It.IsAny<CreateAlertDto>()))
            .ReturnsAsync(new AlertDto());

        // Act
        var result = await _weatherRecordService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        _mockAlertService.Verify(s => s.CreateAsync(It.Is<CreateAlertDto>(a =>
            a.AlertType == "Temperature" && a.Severity == "High")), Times.Once);
    }

    [Test]
    public async Task CreateAsync_WithExtremeCold_CreatesAlert()
    {
        // Arrange
        var cityId = 1;
        var createDto = new CreateWeatherRecordDto
        {
            CityId = cityId,
            Temperature = -25m, // Extreme cold
            Humidity = 50
        };

        var city = new City { Id = cityId, Name = "Test City", Country = "Test", Latitude = 0, Longitude = 0, CreatedAt = DateTime.UtcNow };

        _mockCityRepository.Setup(r => r.GetByIdAsync(cityId))
            .ReturnsAsync(city);
        _mockWeatherRecordRepository.Setup(r => r.AddAsync(It.IsAny<WeatherRecord>()))
            .ReturnsAsync((WeatherRecord w) => w);
        _mockAlertService.Setup(s => s.CreateAsync(It.IsAny<CreateAlertDto>()))
            .ReturnsAsync(new AlertDto());

        // Act
        var result = await _weatherRecordService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        _mockAlertService.Verify(s => s.CreateAsync(It.Is<CreateAlertDto>(a =>
            a.AlertType == "Temperature" && a.Severity == "High")), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WhenRecordExists_ReturnsTrue()
    {
        // Arrange
        var recordId = 1;
        _mockWeatherRecordRepository.Setup(r => r.DeleteAsync(recordId))
            .ReturnsAsync(true);

        // Act
        var result = await _weatherRecordService.DeleteAsync(recordId);

        // Assert
        result.Should().BeTrue();
        _mockWeatherRecordRepository.Verify(r => r.DeleteAsync(recordId), Times.Once);
    }
}
