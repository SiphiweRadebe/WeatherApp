using System.Net.Http.Json;
using WeatherApp.Web.Models;

namespace WeatherApp.Web.Services
{
    public interface IWeatherApiClient
    {
        Task<List<CityViewModel>> GetAllCitiesAsync();
        Task<CityViewModel?> GetCityByIdAsync(int id);
        Task<CityViewModel?> CreateCityAsync(CreateCityRequest request);
        Task<List<WeatherRecordViewModel>> GetWeatherRecordsByCityAsync(int cityId);
        Task<WeatherRecordViewModel?> GetLatestWeatherAsync(int cityId);
        Task<WeatherRecordViewModel?> CreateWeatherRecordAsync(CreateWeatherRecordRequest request);
        Task<List<AlertViewModel>> GetActiveAlertsAsync();
        Task<List<AlertViewModel>> GetAlertsByCityAsync(int cityId);
        Task<WeatherRecordViewModel?> FetchAndSaveWeatherFromOpenWeatherAsync(int cityId);
        Task<OpenWeatherData?> GetCurrentWeatherFromOpenWeatherAsync(int cityId);
    }

    public class WeatherApiClient : IWeatherApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherApiClient> _logger;

        public WeatherApiClient(HttpClient httpClient, ILogger<WeatherApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<CityViewModel>> GetAllCitiesAsync()
        {
            try
            {
                var cities = await _httpClient.GetFromJsonAsync<List<CityViewModel>>("api/cities");
                return cities ?? new List<CityViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cities");
                return new List<CityViewModel>();
            }
        }

        public async Task<CityViewModel?> GetCityByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CityViewModel>($"api/cities/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching city {CityId}", id);
                return null;
            }
        }

        public async Task<CityViewModel?> CreateCityAsync(CreateCityRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/cities", request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API error creating city: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"API error creating city: {response.StatusCode} - {errorContent}");
                }

                var created = await response.Content.ReadFromJsonAsync<CityViewModel>();
                return created;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating city: {Message}", ex.Message);
                throw new HttpRequestException("Error creating city", ex);
            }
        }

        public async Task<List<WeatherRecordViewModel>> GetWeatherRecordsByCityAsync(int cityId)
        {
            try
            {
                var records = await _httpClient.GetFromJsonAsync<List<WeatherRecordViewModel>>($"api/weatherrecords/city/{cityId}");
                return records ?? new List<WeatherRecordViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather records for city {CityId}", cityId);
                return new List<WeatherRecordViewModel>();
            }
        }

        public async Task<WeatherRecordViewModel?> GetLatestWeatherAsync(int cityId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<WeatherRecordViewModel>($"api/weatherrecords/city/{cityId}/latest");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching latest weather for city {CityId}", cityId);
                return null;
            }
        }

        public async Task<WeatherRecordViewModel?> CreateWeatherRecordAsync(CreateWeatherRecordRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/weatherrecords", request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API error creating weather record: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"API error creating weather record: {response.StatusCode} - {errorContent}");
                }
                return await response.Content.ReadFromJsonAsync<WeatherRecordViewModel>();
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating weather record: {Message}", ex.Message);
                throw new HttpRequestException("Error creating weather record", ex);
            }
        }

        public async Task<List<AlertViewModel>> GetActiveAlertsAsync()
        {
            try
            {
                var alerts = await _httpClient.GetFromJsonAsync<List<AlertViewModel>>("api/alerts/active");
                return alerts ?? new List<AlertViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active alerts");
                return new List<AlertViewModel>();
            }
        }

        public async Task<List<AlertViewModel>> GetAlertsByCityAsync(int cityId)
        {
            try
            {
                var alerts = await _httpClient.GetFromJsonAsync<List<AlertViewModel>>($"api/alerts/city/{cityId}");
                return alerts ?? new List<AlertViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching alerts for city {CityId}", cityId);
                return new List<AlertViewModel>();
            }
        }

        public async Task<WeatherRecordViewModel?> FetchAndSaveWeatherFromOpenWeatherAsync(int cityId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<object>($"api/weather/fetch-and-save/{cityId}", new object());
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API error fetching weather: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"API error fetching weather: {response.StatusCode} - {errorContent}");
                }
                return await response.Content.ReadFromJsonAsync<WeatherRecordViewModel>();
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching and saving weather from OpenWeather for city {CityId}: {Message}", cityId, ex.Message);
                throw new HttpRequestException("Error fetching and saving weather", ex);
            }
        }

        public async Task<OpenWeatherData?> GetCurrentWeatherFromOpenWeatherAsync(int cityId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<OpenWeatherData>($"api/weather/current/{cityId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching current weather from OpenWeather for city {CityId}", cityId);
                return null;
            }
        }
    }

    public class OpenWeatherData
    {
        public decimal Temperature { get; set; }
        public decimal FeelsLike { get; set; }
        public int Humidity { get; set; }
        public decimal Pressure { get; set; }
        public decimal WindSpeed { get; set; }
        public decimal? WindDegree { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}