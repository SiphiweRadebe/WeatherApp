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
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CityViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating city");
                return null;
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
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<WeatherRecordViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating weather record");
                return null;
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
    }
}