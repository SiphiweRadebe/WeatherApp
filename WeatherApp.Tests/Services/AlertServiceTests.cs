using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherApp.Core.DTOs;
using WeatherApp.Core.Exceptions;
using WeatherApp.Core.Services;
using WeatherApp.Data.Entities;
using WeatherApp.Data.Repositories;

namespace WeatherApp.Tests.Services;

[TestFixture]
public class AlertServiceTests
{
    private Mock<IAlertRepository> _mockAlertRepository;
    private Mock<ICityRepository> _mockCityRepository;
    private Mock<ILogger<AlertService>> _mockLogger;
    private AlertService _alertService;

    [SetUp]
    public void SetUp()
    {
        _mockAlertRepository = new Mock<IAlertRepository>();
        _mockCityRepository = new Mock<ICityRepository>();
        _mockLogger = new Mock<ILogger<AlertService>>();
        _alertService = new AlertService(
            _mockAlertRepository.Object,
            _mockCityRepository.Object,
            _mockLogger.Object);
    }

    [Test]
    public async Task GetByIdAsync_WhenAlertExists_ReturnsAlertDto()
    {
        // Arrange
        var alertId = 1;
        var alert = new Alert
        {
            Id = alertId,
            Title = "Heat Wave",
            Description = "Extreme temperatures expected",
            Severity = "High",
            AlertType = "Temperature",
            StartTime = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CityAlerts = new List<CityAlert>()
        };

        _mockAlertRepository.Setup(r => r.GetAlertWithCitiesAsync(alertId))
            .ReturnsAsync(alert);

        // Act
        var result = await _alertService.GetByIdAsync(alertId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(alertId);
        result.Title.Should().Be("Heat Wave");
        result.Severity.Should().Be("High");
    }

    [Test]
    public void GetByIdAsync_WhenAlertDoesNotExist_ThrowsEntityNotFoundException()
    {
        // Arrange
        var alertId = 999;
        _mockAlertRepository.Setup(r => r.GetAlertWithCitiesAsync(alertId))
            .ReturnsAsync((Alert?)null);

        // Act & Assert
        Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _alertService.GetByIdAsync(alertId));
    }

    [Test]
    public async Task GetAllActiveAsync_ReturnsActiveAlerts()
    {
        // Arrange
        var alerts = new List<Alert>
        {
            new Alert { Id = 1, Title = "Alert 1", Severity = "High", AlertType = "Storm", IsActive = true, StartTime = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new Alert { Id = 2, Title = "Alert 2", Severity = "Medium", AlertType = "Wind", IsActive = true, StartTime = DateTime.UtcNow, CreatedAt = DateTime.UtcNow }
        };

        _mockAlertRepository.Setup(r => r.GetActiveAlertsAsync())
            .ReturnsAsync(alerts);

        // Act
        var result = await _alertService.GetAllActiveAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.IsActive);
    }

    [Test]
    public async Task GetByCityIdAsync_ReturnsAlertsForCity()
    {
        // Arrange
        var cityId = 1;
        var alerts = new List<Alert>
        {
            new Alert { Id = 1, Title = "City Alert", Severity = "Low", AlertType = "Other", IsActive = true, StartTime = DateTime.UtcNow, CreatedAt = DateTime.UtcNow }
        };

        _mockAlertRepository.Setup(r => r.GetAlertsByCityIdAsync(cityId))
            .ReturnsAsync(alerts);

        // Act
        var result = await _alertService.GetByCityIdAsync(cityId);

        // Assert
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("City Alert");
    }

    [Test]
    public async Task CreateAsync_WithValidData_CreatesAlert()
    {
        // Arrange
        var createDto = new CreateAlertDto
        {
            Title = "Storm Warning",
            Description = "Severe storm approaching",
            Severity = "High",
            AlertType = "Storm",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(12)
        };

        _mockAlertRepository.Setup(r => r.AddAsync(It.IsAny<Alert>()))
            .ReturnsAsync((Alert a) => a);

        // Act
        var result = await _alertService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Storm Warning");
        result.Severity.Should().Be("High");
        result.IsActive.Should().BeTrue();
        _mockAlertRepository.Verify(r => r.AddAsync(It.IsAny<Alert>()), Times.Once);
    }

    [Test]
    public void CreateAsync_WithInvalidSeverity_ThrowsBusinessException()
    {
        // Arrange
        var createDto = new CreateAlertDto
        {
            Title = "Test Alert",
            Description = "Test",
            Severity = "Invalid",
            AlertType = "Storm"
        };

        // Act & Assert
        Assert.ThrowsAsync<BusinessException>(async () =>
            await _alertService.CreateAsync(createDto));
    }

    [Test]
    public void CreateAsync_WithInvalidAlertType_ThrowsBusinessException()
    {
        // Arrange
        var createDto = new CreateAlertDto
        {
            Title = "Test Alert",
            Description = "Test",
            Severity = "High",
            AlertType = "InvalidType"
        };

        // Act & Assert
        Assert.ThrowsAsync<BusinessException>(async () =>
            await _alertService.CreateAsync(createDto));
    }

    [Test]
    public async Task CreateAsync_WithCityIds_AssociatesAlertWithCities()
    {
        // Arrange
        var cityId = 1;
        var createDto = new CreateAlertDto
        {
            Title = "Test Alert",
            Description = "Test",
            Severity = "Medium",
            AlertType = "Temperature",
            CityIds = new List<int> { cityId }
        };

        var city = new City { Id = cityId, Name = "Test City", Country = "Test", Latitude = 0, Longitude = 0, CreatedAt = DateTime.UtcNow };

        _mockAlertRepository.Setup(r => r.AddAsync(It.IsAny<Alert>()))
            .ReturnsAsync((Alert a) => a);
        _mockCityRepository.Setup(r => r.GetByIdAsync(cityId))
            .ReturnsAsync(city);
        _mockAlertRepository.Setup(r => r.AssociateAlertWithCityAsync(cityId, It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _alertService.CreateAsync(createDto);

        // Assert
        _mockAlertRepository.Verify(r => r.AssociateAlertWithCityAsync(cityId, It.IsAny<int>()), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WithValidData_UpdatesAlert()
    {
        // Arrange
        var alertId = 1;
        var existingAlert = new Alert
        {
            Id = alertId,
            Title = "Old Title",
            Description = "Old Description",
            Severity = "Low",
            AlertType = "Other",
            IsActive = true,
            StartTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        var updateDto = new UpdateAlertDto
        {
            Title = "New Title",
            Severity = "High",
            IsActive = false
        };

        _mockAlertRepository.Setup(r => r.GetByIdAsync(alertId))
            .ReturnsAsync(existingAlert);
        _mockAlertRepository.Setup(r => r.UpdateAsync(It.IsAny<Alert>()))
            .ReturnsAsync((Alert a) => a);

        // Act
        var result = await _alertService.UpdateAsync(alertId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Title");
        result.Severity.Should().Be("High");
        result.IsActive.Should().BeFalse();
        _mockAlertRepository.Verify(r => r.UpdateAsync(It.IsAny<Alert>()), Times.Once);
    }

    [Test]
    public void UpdateAsync_WhenAlertDoesNotExist_ThrowsEntityNotFoundException()
    {
        // Arrange
        var alertId = 999;
        var updateDto = new UpdateAlertDto { Title = "Test" };

        _mockAlertRepository.Setup(r => r.GetByIdAsync(alertId))
            .ReturnsAsync((Alert?)null);

        // Act & Assert
        Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _alertService.UpdateAsync(alertId, updateDto));
    }

    [Test]
    public async Task DeleteAsync_WhenAlertExists_ReturnsTrue()
    {
        // Arrange
        var alertId = 1;
        _mockAlertRepository.Setup(r => r.DeleteAsync(alertId))
            .ReturnsAsync(true);

        // Act
        var result = await _alertService.DeleteAsync(alertId);

        // Assert
        result.Should().BeTrue();
        _mockAlertRepository.Verify(r => r.DeleteAsync(alertId), Times.Once);
    }
}
