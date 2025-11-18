using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherApp.Core.IService;

namespace WeatherApp.Core.Services
{
    public class OpenWeatherClient : IOpenWeatherClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenWeatherClient> _logger;
        private readonly string ApiKey;
        private readonly string _baseUrl;

        public OpenWeatherClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<OpenWeatherClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            ApiKey = configuration["OpenWeatherMap:ApiKey"]
                ?? throw new InvalidOperationException("OpenWeather API key not configured");
            _baseUrl = configuration["OpenWeather:BaseUrl"]
                ?? "https://api.openweathermap.org/data/2.5";
        }

        public async Task<OpenWeatherResponse?> GetCurrentWeatherAsync(decimal latitude, decimal longitude)
        {
            try
            {
                var url = $"{_baseUrl}/weather?lat={latitude}&lon={longitude}&appid={ApiKey}&units=metric";

                _logger.LogInformation("Calling OpenWeather API for coordinates: {Lat}, {Lon}", latitude, longitude);

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("OpenWeather API error: Status {StatusCode}, Response: {Response}",
                        response.StatusCode, errorContent);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var main = root.GetProperty("main");
                var wind = root.GetProperty("wind");
                var weather = root.GetProperty("weather")[0];

                var result = new OpenWeatherResponse
                {
                    Temperature = main.GetProperty("temp").GetDecimal(),
                    FeelsLike = main.GetProperty("feels_like").GetDecimal(),
                    Humidity = main.GetProperty("humidity").GetInt32(),
                    Pressure = main.GetProperty("pressure").GetDecimal(),
                    WindSpeed = wind.GetProperty("speed").GetDecimal() * 3.6m, // Convert m/s to km/h
                    WindDegree = wind.TryGetProperty("deg", out var deg) ? deg.GetInt32() : 0,
                    Condition = weather.GetProperty("main").GetString() ?? "Unknown",
                    Description = weather.GetProperty("description").GetString() ?? ""
                };

                _logger.LogInformation("Successfully fetched weather: {Temp}°C, {Condition}",
                    result.Temperature, result.Condition);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather from OpenWeather API");
                return null;
            }
        }
    }
}