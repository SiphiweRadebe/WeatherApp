using Microsoft.AspNetCore.Mvc;
using WeatherApp.Core.DTOs;
using WeatherApp.Core.IService;
using Microsoft.Extensions.Logging;

namespace WeatherApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IOpenWeatherClient _openWeatherClient;
        private readonly IWeatherRecordService _weatherRecordService;
        private readonly ICityService _cityService;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(
            IOpenWeatherClient openWeatherClient,
            IWeatherRecordService weatherRecordService,
            ICityService cityService,
            ILogger<WeatherController> logger)
        {
            _openWeatherClient = openWeatherClient;
            _weatherRecordService = weatherRecordService;
            _cityService = cityService;
            _logger = logger;
        }

        /// <summary>
        /// Fetch current weather from OpenWeatherMap and save to database
        /// </summary>
        [HttpPost("fetch-and-save/{cityId}")]
        public async Task<ActionResult<WeatherRecordDto>> FetchAndSaveWeatherAsync(int cityId)
        {
            try
            {
                // Get city details
                var city = await _cityService.GetByIdAsync(cityId);
                if (city == null)
                    return NotFound($"City with ID {cityId} not found");

                // Fetch from OpenWeatherMap
                var weatherData = await _openWeatherClient.GetCurrentWeatherAsync(city.Latitude, city.Longitude);
                if (weatherData == null)
                    return StatusCode(500, "Failed to fetch weather data from OpenWeatherMap");

                // Create weather record DTO
                var createDto = new CreateWeatherRecordDto
                {
                    CityId = cityId,
                    ObservationTime = DateTime.UtcNow,
                    Temperature = weatherData.Temperature,
                    FeelsLike = weatherData.FeelsLike,
                    Humidity = weatherData.Humidity,
                    WindSpeed = weatherData.WindSpeed,
                    WindDirection = GetWindDirectionFromDegree(weatherData.WindDegree),
                    Pressure = weatherData.Pressure,
                    Condition = weatherData.Condition,
                    Description = weatherData.Description
                };

                // Save to database
                var savedRecord = await _weatherRecordService.CreateAsync(createDto);

                _logger.LogInformation("Successfully fetched and saved weather for city {CityId}", cityId);
                return Ok(savedRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching and saving weather for city {CityId}", cityId);
                return StatusCode(500, "An error occurred while fetching weather data");
            }
        }

        /// <summary>
        /// Fetch current weather from OpenWeatherMap only (without saving)
        /// </summary>
        [HttpGet("current/{cityId}")]
        public async Task<ActionResult> GetCurrentWeatherAsync(int cityId)
        {
            try
            {
                // Get city details
                var city = await _cityService.GetByIdAsync(cityId);
                if (city == null)
                    return NotFound($"City with ID {cityId} not found");

                // Fetch from OpenWeatherMap
                var weatherData = await _openWeatherClient.GetCurrentWeatherAsync(city.Latitude, city.Longitude);
                if (weatherData == null)
                    return StatusCode(500, "Failed to fetch weather data from OpenWeatherMap");

                return Ok(weatherData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching current weather for city {CityId}", cityId);
                return StatusCode(500, "An error occurred while fetching weather data");
            }
        }

        private static string GetWindDirectionFromDegree(decimal? degrees)
        {
            if (!degrees.HasValue)
                return "Unknown";

            var normalized = (degrees.Value % 360 + 360) % 360;
            var directions = new[] { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };
            var index = (int)((normalized + 11.25m) / 22.5m) % 16;
            return directions[index];
        }
    }
}
