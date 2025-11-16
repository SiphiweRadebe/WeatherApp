using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WeatherApp.Core.IService;

namespace WeatherApp.Core.Services
{
    public class OpenWeatherClient : IOpenWeatherClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenWeatherClient> _logger;
        private readonly string _apiKey;

        // OpenWeatherMap API base URL
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

        public OpenWeatherClient(HttpClient httpClient, ILogger<OpenWeatherClient> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["OpenWeatherMap:ApiKey"] ?? throw new InvalidOperationException("OpenWeatherMap API key is not configured");
        }

        public async Task<OpenWeatherResponse?> GetCurrentWeatherAsync(decimal latitude, decimal longitude)
        {
            try
            {
                _logger.LogInformation("Fetching weather from OpenWeatherMap for lat={Latitude}, lon={Longitude}", latitude, longitude);

                var url = $"{BaseUrl}?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("OpenWeatherMap API returned status code {StatusCode}", response.StatusCode);
                    return null;
                }

                var data = await response.Content.ReadFromJsonAsync<OpenWeatherMapRawResponse>();
                if (data == null)
                {
                    _logger.LogWarning("Failed to deserialize OpenWeatherMap response");
                    return null;
                }

                var windDirection = GetWindDirection(data.Wind?.Deg ?? 0);

                return new OpenWeatherResponse
                {
                    Temperature = data.Main?.Temp ?? 0,
                    FeelsLike = data.Main?.FeelsLike ?? 0,
                    Humidity = data.Main?.Humidity ?? 0,
                    Pressure = data.Main?.Pressure ?? 0,
                    WindSpeed = data.Wind?.Speed ?? 0,
                    WindDegree = data.Wind?.Deg,
                    Condition = data.Weather?.FirstOrDefault()?.Main ?? "Unknown",
                    Description = data.Weather?.FirstOrDefault()?.Description ?? "No description"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather from OpenWeatherMap");
                return null;
            }
        }

        /// <summary>
        /// Convert wind degree to cardinal direction
        /// </summary>
        private static string GetWindDirection(decimal degrees)
        {
            var normalized = (degrees % 360 + 360) % 360;
            var directions = new[] { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };
            var index = (int)((normalized + 11.25m) / 22.5m) % 16;
            return directions[index];
        }

        #region Internal DTOs for OpenWeatherMap Response Mapping

        private class OpenWeatherMapRawResponse
        {
            [JsonPropertyName("main")]
            public MainInfo? Main { get; set; }

            [JsonPropertyName("weather")]
            public Weather[]? Weather { get; set; }

            [JsonPropertyName("wind")]
            public WindInfo? Wind { get; set; }
        }

        private class MainInfo
        {
            [JsonPropertyName("temp")]
            public decimal Temp { get; set; }

            [JsonPropertyName("feels_like")]
            public decimal FeelsLike { get; set; }

            [JsonPropertyName("humidity")]
            public int Humidity { get; set; }

            [JsonPropertyName("pressure")]
            public decimal Pressure { get; set; }
        }

        private class Weather
        {
            [JsonPropertyName("main")]
            public string? Main { get; set; }

            [JsonPropertyName("description")]
            public string? Description { get; set; }
        }

        private class WindInfo
        {
            [JsonPropertyName("speed")]
            public decimal Speed { get; set; }

            [JsonPropertyName("deg")]
            public decimal Deg { get; set; }
        }

        #endregion
    }
}
