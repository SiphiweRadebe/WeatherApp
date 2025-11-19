using System.Net;
using System.Net.Http.Json;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.HttpClient;
using WeatherApp.Tests.Fakers;
using WeatherApp.Web.Models;
using WeatherApp.Web.Services;

namespace WeatherApp.Tests.ServiceTests;

[TestFixture]
public class WeatherApiClientTests
{
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private HttpClient _httpClient;
    private Mock<ILogger<WeatherApiClient>> _loggerMock;
    private WeatherApiClient _weatherApiClient;
    private Faker _faker;

    [SetUp]
    public void SetUp()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = _httpMessageHandlerMock.CreateClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5000/");
        _loggerMock = new Mock<ILogger<WeatherApiClient>>();
        _weatherApiClient = new WeatherApiClient(_httpClient, _loggerMock.Object);
        _faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }

    #region GetAllCitiesAsync Tests

    [Test]
    public async Task GetAllCitiesAsync_WhenSuccessful_ReturnsCities()
    {
        // Arrange
        var expectedCities = WeatherFakers.CityViewModelFaker.Generate(3);
        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Get, "http://localhost:5000/api/cities")
            .ReturnsJsonResponse(HttpStatusCode.OK, expectedCities);

        // Act
        var result = await _weatherApiClient.GetAllCitiesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(expectedCities);
    }

    [Test]
    public async Task GetAllCitiesAsync_WhenApiReturnsNull_ReturnsEmptyList()
    {
        // Arrange
        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Get, "http://localhost:5000/api/cities")
            .ReturnsJsonResponse<List<CityViewModel>>(HttpStatusCode.OK, null!);

        // Act
        var result = await _weatherApiClient.GetAllCitiesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetAllCitiesAsync_WhenExceptionOccurs_ReturnsEmptyList()
    {
        // Arrange
        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Get, "http://localhost:5000/api/cities")
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var result = await _weatherApiClient.GetAllCitiesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region GetCityByIdAsync Tests

    [Test]
    public async Task GetCityByIdAsync_WhenCityExists_ReturnsCity()
    {
        // Arrange
        var cityId = _faker.Random.Int(1, 1000);
        var expectedCity = WeatherFakers.CityViewModelFaker.Generate();
        expectedCity.Id = cityId;

        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Get, $"http://localhost:5000/api/cities/{cityId}")
            .ReturnsJsonResponse(HttpStatusCode.OK, expectedCity);

        // Act
        var result = await _weatherApiClient.GetCityByIdAsync(cityId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedCity);
    }

    [Test]
    public async Task GetCityByIdAsync_WhenCityNotFound_ReturnsNull()
    {
        // Arrange
        var cityId = _faker.Random.Int(1, 1000);
        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Get, $"http://localhost:5000/api/cities/{cityId}")
            .ReturnsResponse(HttpStatusCode.NotFound);

        // Act
        var result = await _weatherApiClient.GetCityByIdAsync(cityId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateCityAsync Tests

    [Test]
    public async Task CreateCityAsync_WhenSuccessful_ReturnsCreatedCity()
    {
        // Arrange
        var request = WeatherFakers.CreateCityRequestFaker.Generate();
        var expectedCity = WeatherFakers.CityViewModelFaker.Generate();

        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Post, "http://localhost:5000/api/cities")
            .ReturnsJsonResponse(HttpStatusCode.Created, expectedCity);

        // Act
        var result = await _weatherApiClient.CreateCityAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedCity);
    }

    [Test]
    public async Task CreateCityAsync_WhenApiFails_ThrowsHttpRequestException()
    {
        // Arrange
        var request = WeatherFakers.CreateCityRequestFaker.Generate();
        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Post, "http://localhost:5000/api/cities")
            .ReturnsResponse(HttpStatusCode.BadRequest, "Validation error");

        // Act
        var act = async () => await _weatherApiClient.CreateCityAsync(request);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("*API error creating city*");
    }

    #endregion

    #region GetWeatherRecordsByCityAsync Tests

    [Test]
    public async Task GetWeatherRecordsByCityAsync_WhenSuccessful_ReturnsWeatherRecords()
    {
        // Arrange
        var cityId = _faker.Random.Int(1, 1000);
        var expectedRecords = WeatherFakers.WeatherRecordViewModelFaker.Generate(5);

        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Get, $"http://localhost:5000/api/weatherrecords/city/{cityId}")
            .ReturnsJsonResponse(HttpStatusCode.OK, expectedRecords);

        // Act
        var result = await _weatherApiClient.GetWeatherRecordsByCityAsync(cityId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
        result.Should().BeEquivalentTo(expectedRecords);
    }

    [Test]
    public async Task GetWeatherRecordsByCityAsync_WhenNoRecords_ReturnsEmptyList()
    {
        // Arrange
        var cityId = _faker.Random.Int(1, 1000);
        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Get, $"http://localhost:5000/api/weatherrecords/city/{cityId}")
            .ReturnsJsonResponse(HttpStatusCode.OK, new List<WeatherRecordViewModel>());

        // Act
        var result = await _weatherApiClient.GetWeatherRecordsByCityAsync(cityId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region GetLatestWeatherAsync Tests

    [Test]
    public async Task GetLatestWeatherAsync_WhenRecordExists_ReturnsLatestWeather()
    {
        // Arrange
        var cityId = _faker.Random.Int(1, 1000);
        var expectedRecord = WeatherFakers.WeatherRecordViewModelFaker.Generate();
        expectedRecord.CityId = cityId;

        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Get, $"http://localhost:5000/api/weatherrecords/city/{cityId}/latest")
            .ReturnsJsonResponse(HttpStatusCode.OK, expectedRecord);

        // Act
        var result = await _weatherApiClient.GetLatestWeatherAsync(cityId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedRecord);
    }

    [Test]
    public async Task GetLatestWeatherAsync_WhenNoRecord_ReturnsNull()
    {
        // Arrange
        var cityId = _faker.Random.Int(1, 1000);
        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Get, $"http://localhost:5000/api/weatherrecords/city/{cityId}/latest")
            .ReturnsResponse(HttpStatusCode.NotFound);

        // Act
        var result = await _weatherApiClient.GetLatestWeatherAsync(cityId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateWeatherRecordAsync Tests

    [Test]
    public async Task CreateWeatherRecordAsync_WhenSuccessful_ReturnsCreatedRecord()
    {
        // Arrange
        var request = WeatherFakers.CreateWeatherRecordRequestFaker.Generate();
        var expectedRecord = WeatherFakers.WeatherRecordViewModelFaker.Generate();

        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Post, "http://localhost:5000/api/weatherrecords")
            .ReturnsJsonResponse(HttpStatusCode.Created, expectedRecord);

        // Act
        var result = await _weatherApiClient.CreateWeatherRecordAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedRecord);
    }

    [Test]
    public async Task CreateWeatherRecordAsync_WhenApiFails_ThrowsHttpRequestException()
    {
        // Arrange
        var request = WeatherFakers.CreateWeatherRecordRequestFaker.Generate();
        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Post, "http://localhost:5000/api/weatherrecords")
            .ReturnsResponse(HttpStatusCode.BadRequest, "Invalid data");

        // Act
        var act = async () => await _weatherApiClient.CreateWeatherRecordAsync(request);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("*API error creating weather record*");
    }

    #endregion

    #region GetActiveAlertsAsync Tests

    [Test]
    public async Task GetActiveAlertsAsync_WhenAlertsExist_ReturnsAlerts()
    {
        // Arrange
        var expectedAlerts = WeatherFakers.AlertViewModelFaker.Generate(4);

        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Get, "http://localhost:5000/api/alerts/active")
            .ReturnsJsonResponse(HttpStatusCode.OK, expectedAlerts);

        // Act
        var result = await _weatherApiClient.GetActiveAlertsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4);
        result.Should().BeEquivalentTo(expectedAlerts);
    }

    [Test]
    public async Task GetActiveAlertsAsync_WhenNoAlerts_ReturnsEmptyList()
    {
        // Arrange
        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Get, "http://localhost:5000/api/alerts/active")
            .ReturnsJsonResponse(HttpStatusCode.OK, new List<AlertViewModel>());

        // Act
        var result = await _weatherApiClient.GetActiveAlertsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region GetAlertsByCityAsync Tests

    [Test]
    public async Task GetAlertsByCityAsync_WhenAlertsExist_ReturnsAlerts()
    {
        // Arrange
        var cityId = _faker.Random.Int(1, 1000);
        var expectedAlerts = WeatherFakers.AlertViewModelFaker.Generate(2);

        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Get, $"http://localhost:5000/api/alerts/city/{cityId}")
            .ReturnsJsonResponse(HttpStatusCode.OK, expectedAlerts);

        // Act
        var result = await _weatherApiClient.GetAlertsByCityAsync(cityId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expectedAlerts);
    }

    #endregion

    #region FetchAndSaveWeatherFromOpenWeatherAsync Tests

    [Test]
    public async Task FetchAndSaveWeatherFromOpenWeatherAsync_WhenSuccessful_ReturnsWeatherRecord()
    {
        // Arrange
        var cityId = _faker.Random.Int(1, 1000);
        var expectedRecord = WeatherFakers.WeatherRecordViewModelFaker.Generate();

        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Post, $"http://localhost:5000/api/weather/fetch-and-save/{cityId}")
            .ReturnsJsonResponse(HttpStatusCode.OK, expectedRecord);

        // Act
        var result = await _weatherApiClient.FetchAndSaveWeatherFromOpenWeatherAsync(cityId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedRecord);
    }

    [Test]
    public async Task FetchAndSaveWeatherFromOpenWeatherAsync_WhenApiFails_ThrowsHttpRequestException()
    {
        // Arrange
        var cityId = _faker.Random.Int(1, 1000);
        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Post, $"http://localhost:5000/api/weather/fetch-and-save/{cityId}")
            .ReturnsResponse(HttpStatusCode.InternalServerError, "OpenWeather API error");

        // Act
        var act = async () => await _weatherApiClient.FetchAndSaveWeatherFromOpenWeatherAsync(cityId);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("*API error fetching weather*");
    }

    #endregion

    #region GetCurrentWeatherFromOpenWeatherAsync Tests

    [Test]
    public async Task GetCurrentWeatherFromOpenWeatherAsync_WhenSuccessful_ReturnsOpenWeatherData()
    {
        // Arrange
        var cityId = _faker.Random.Int(1, 1000);
        var expectedData = WeatherFakers.OpenWeatherDataFaker.Generate();

        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Get, $"http://localhost:5000/api/weather/current/{cityId}")
            .ReturnsJsonResponse(HttpStatusCode.OK, expectedData);

        // Act
        var result = await _weatherApiClient.GetCurrentWeatherFromOpenWeatherAsync(cityId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedData);
    }

    [Test]
    public async Task GetCurrentWeatherFromOpenWeatherAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        var cityId = _faker.Random.Int(1, 1000);
        _httpMessageHandlerMock
            .SetupRequest(HttpMethod.Get, $"http://localhost:5000/api/weather/current/{cityId}")
            .ReturnsResponse(HttpStatusCode.NotFound);

        // Act
        var result = await _weatherApiClient.GetCurrentWeatherFromOpenWeatherAsync(cityId);

        // Assert
        result.Should().BeNull();
    }

    #endregion
}
