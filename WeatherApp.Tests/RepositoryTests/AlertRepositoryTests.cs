using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data.Context;
using WeatherApp.Data.Entities;
using WeatherApp.Data.Repositories;

namespace WeatherApp.Tests.RepositoryTests;

[TestFixture]
public class AlertRepositoryTests
{
    private WeatherDbContext _context;
    private AlertRepository _alertRepository;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<WeatherDbContext>()
            .UseInMemoryDatabase(databaseName: $"AlertTestDb_{Guid.NewGuid()}")
            .Options;

        _context = new WeatherDbContext(options);
        _alertRepository = new AlertRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetByIdAsync_WhenAlertExists_ReturnsAlert()
    {
        // Arrange
        var alert = new Alert
        {
            Title = "Storm Warning",
            Description = "Severe storm approaching",
            Severity = "High",
            AlertType = "Storm",
            StartTime = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Alerts.Add(alert);
        await _context.SaveChangesAsync();

        // Act
        var result = await _alertRepository.GetByIdAsync(alert.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Storm Warning");
        result.Severity.Should().Be("High");
    }

    [Test]
    public async Task GetActiveAlertsAsync_ReturnsOnlyActiveAlerts()
    {
        // Arrange
        var alerts = new[]
        {
            new Alert { Title = "Active Alert 1", Severity = "High", AlertType = "Storm", StartTime = DateTime.UtcNow, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Alert { Title = "Inactive Alert", Severity = "Low", AlertType = "Other", StartTime = DateTime.UtcNow, IsActive = false, CreatedAt = DateTime.UtcNow },
            new Alert { Title = "Active Alert 2", Severity = "Medium", AlertType = "Wind", StartTime = DateTime.UtcNow, IsActive = true, CreatedAt = DateTime.UtcNow }
        };
        _context.Alerts.AddRange(alerts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _alertRepository.GetActiveAlertsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.IsActive);
    }

    [Test]
    public async Task GetAlertsByCityIdAsync_ReturnsAlertsForSpecificCity()
    {
        // Arrange
        var city1 = new City { Name = "New York", Country = "USA", Latitude = 40.7128m, Longitude = -74.0060m, CreatedAt = DateTime.UtcNow };
        var city2 = new City { Name = "London", Country = "UK", Latitude = 51.5074m, Longitude = -0.1278m, CreatedAt = DateTime.UtcNow };
        _context.Cities.AddRange(city1, city2);
        await _context.SaveChangesAsync();

        var alert1 = new Alert { Title = "Alert 1", Severity = "High", AlertType = "Temperature", StartTime = DateTime.UtcNow, IsActive = true, CreatedAt = DateTime.UtcNow };
        var alert2 = new Alert { Title = "Alert 2", Severity = "Medium", AlertType = "Storm", StartTime = DateTime.UtcNow, IsActive = true, CreatedAt = DateTime.UtcNow };
        var alert3 = new Alert { Title = "Alert 3", Severity = "Low", AlertType = "Other", StartTime = DateTime.UtcNow, IsActive = true, CreatedAt = DateTime.UtcNow };
        _context.Alerts.AddRange(alert1, alert2, alert3);
        await _context.SaveChangesAsync();

        var cityAlert1 = new CityAlert { CityId = city1.Id, AlertId = alert1.Id, AssociatedAt = DateTime.UtcNow, NotificationSent = false };
        var cityAlert2 = new CityAlert { CityId = city1.Id, AlertId = alert2.Id, AssociatedAt = DateTime.UtcNow, NotificationSent = false };
        var cityAlert3 = new CityAlert { CityId = city2.Id, AlertId = alert3.Id, AssociatedAt = DateTime.UtcNow, NotificationSent = false };
        _context.CityAlerts.AddRange(cityAlert1, cityAlert2, cityAlert3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _alertRepository.GetAlertsByCityIdAsync(city1.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(a => a.Title == "Alert 1");
        result.Should().Contain(a => a.Title == "Alert 2");
    }

    [Test]
    public async Task GetAlertsByTypeAsync_ReturnsAlertsOfSpecificType()
    {
        // Arrange
        var alerts = new[]
        {
            new Alert { Title = "Heat Wave", Severity = "High", AlertType = "Temperature", StartTime = DateTime.UtcNow, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Alert { Title = "Storm Warning", Severity = "High", AlertType = "Storm", StartTime = DateTime.UtcNow, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Alert { Title = "Cold Front", Severity = "Medium", AlertType = "Temperature", StartTime = DateTime.UtcNow, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Alert { Title = "Old Temperature Alert", Severity = "Low", AlertType = "Temperature", StartTime = DateTime.UtcNow, IsActive = false, CreatedAt = DateTime.UtcNow }
        };
        _context.Alerts.AddRange(alerts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _alertRepository.GetAlertsByTypeAsync("Temperature");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.AlertType == "Temperature" && a.IsActive);
    }

    [Test]
    public async Task GetAlertWithCitiesAsync_ReturnsAlertWithAssociatedCities()
    {
        // Arrange
        var city1 = new City { Name = "New York", Country = "USA", Latitude = 40.7128m, Longitude = -74.0060m, CreatedAt = DateTime.UtcNow };
        var city2 = new City { Name = "Boston", Country = "USA", Latitude = 42.3601m, Longitude = -71.0589m, CreatedAt = DateTime.UtcNow };
        _context.Cities.AddRange(city1, city2);
        await _context.SaveChangesAsync();

        var alert = new Alert
        {
            Title = "Hurricane Warning",
            Severity = "Extreme",
            AlertType = "Storm",
            StartTime = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Alerts.Add(alert);
        await _context.SaveChangesAsync();

        var cityAlerts = new[]
        {
            new CityAlert { CityId = city1.Id, AlertId = alert.Id, AssociatedAt = DateTime.UtcNow, NotificationSent = false },
            new CityAlert { CityId = city2.Id, AlertId = alert.Id, AssociatedAt = DateTime.UtcNow, NotificationSent = false }
        };
        _context.CityAlerts.AddRange(cityAlerts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _alertRepository.GetAlertWithCitiesAsync(alert.Id);

        // Assert
        result.Should().NotBeNull();
        result!.CityAlerts.Should().HaveCount(2);
        result.CityAlerts.Select(ca => ca.City.Name).Should().Contain("New York");
        result.CityAlerts.Select(ca => ca.City.Name).Should().Contain("Boston");
    }

    [Test]
    public async Task AssociateAlertWithCityAsync_CreatesAssociation()
    {
        // Arrange
        var city = new City { Name = "Miami", Country = "USA", Latitude = 25.7617m, Longitude = -80.1918m, CreatedAt = DateTime.UtcNow };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var alert = new Alert
        {
            Title = "Hurricane Alert",
            Severity = "Extreme",
            AlertType = "Storm",
            StartTime = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Alerts.Add(alert);
        await _context.SaveChangesAsync();

        // Act
        await _alertRepository.AssociateAlertWithCityAsync(city.Id, alert.Id);

        // Assert
        var cityAlert = await _context.CityAlerts
            .FirstOrDefaultAsync(ca => ca.CityId == city.Id && ca.AlertId == alert.Id);
        cityAlert.Should().NotBeNull();
        cityAlert!.NotificationSent.Should().BeFalse();
    }

    [Test]
    public async Task RemoveAlertFromCityAsync_RemovesAssociation()
    {
        // Arrange
        var city = new City { Name = "Seattle", Country = "USA", Latitude = 47.6062m, Longitude = -122.3321m, CreatedAt = DateTime.UtcNow };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var alert = new Alert
        {
            Title = "Flood Warning",
            Severity = "High",
            AlertType = "Flood",
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
        await _alertRepository.RemoveAlertFromCityAsync(city.Id, alert.Id);

        // Assert
        var result = await _context.CityAlerts
            .FirstOrDefaultAsync(ca => ca.CityId == city.Id && ca.AlertId == alert.Id);
        result.Should().BeNull();
    }

    [Test]
    public async Task AddAsync_AddsAlert()
    {
        // Arrange
        var alert = new Alert
        {
            Title = "Snow Alert",
            Description = "Heavy snowfall expected",
            Severity = "Medium",
            AlertType = "Snow",
            StartTime = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _alertRepository.AddAsync(alert);

        // Assert
        var result = await _context.Alerts.FindAsync(alert.Id);
        result.Should().NotBeNull();
        result!.Title.Should().Be("Snow Alert");
    }

    [Test]
    public async Task UpdateAsync_UpdatesAlert()
    {
        // Arrange
        var alert = new Alert
        {
            Title = "Wind Alert",
            Description = "Strong winds",
            Severity = "Low",
            AlertType = "Wind",
            StartTime = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Alerts.Add(alert);
        await _context.SaveChangesAsync();

        // Act
        alert.Severity = "High";
        alert.IsActive = false;
        alert.UpdatedAt = DateTime.UtcNow;
        await _alertRepository.UpdateAsync(alert);

        // Assert
        var result = await _context.Alerts.FindAsync(alert.Id);
        result.Should().NotBeNull();
        result!.Severity.Should().Be("High");
        result.IsActive.Should().BeFalse();
        result.UpdatedAt.Should().NotBeNull();
    }

    [Test]
    public async Task DeleteAsync_DeletesAlert()
    {
        // Arrange
        var alert = new Alert
        {
            Title = "Test Alert",
            Severity = "Low",
            AlertType = "Other",
            StartTime = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Alerts.Add(alert);
        await _context.SaveChangesAsync();

        // Act
        var result = await _alertRepository.DeleteAsync(alert.Id);

        // Assert
        result.Should().BeTrue();
        var deletedAlert = await _context.Alerts.FindAsync(alert.Id);
        deletedAlert.Should().BeNull();
    }
}
