using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApp.Core.DTOs;
using WeatherApp.Core.Exceptions;
using WeatherApp.Core.IService;
using WeatherApp.Data.Entities;
using WeatherApp.Data.Repositories;

namespace WeatherApp.Core.Services
{
    public class WeatherRecordService : IWeatherRecordService
    {
        private readonly IWeatherRecordRepository _weatherRecordRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IAlertService _alertService;
        private readonly ILogger<WeatherRecordService> _logger;
        private readonly IOpenWeatherClient _openWeatherClient;

        public WeatherRecordService(
            IWeatherRecordRepository weatherRecordRepository,
            ICityRepository cityRepository,
            IAlertService alertService,
            IOpenWeatherClient openWeatherClient,
            ILogger<WeatherRecordService> logger)
        {
            _weatherRecordRepository = weatherRecordRepository;
            _cityRepository = cityRepository;
            _alertService = alertService;
            _openWeatherClient = openWeatherClient;
            _logger = logger;
        }

        public async Task<WeatherRecordDto> GetByIdAsync(int id)
        {
            var record = await _weatherRecordRepository.GetByIdAsync(id);
            if (record == null)
                throw new EntityNotFoundException($"Weather record with ID {id} not found");

            return MapToDto(record);
        }

        public async Task<IEnumerable<WeatherRecordDto>> GetByCityIdAsync(int cityId)
        {
            var records = await _weatherRecordRepository.GetByCityIdAsync(cityId);
            return records.Select(MapToDto);
        }

        public async Task<WeatherRecordDto?> GetLatestByCityIdAsync(int cityId)
        {
            var record = await _weatherRecordRepository.GetLatestByCityIdAsync(cityId);
            return record != null ? MapToDto(record) : null;
        }

        public async Task<WeatherRecordDto> CreateAsync(CreateWeatherRecordDto dto)
        {
            var city = await _cityRepository.GetByIdAsync(dto.CityId);
            if (city == null)
                throw new EntityNotFoundException($"City with ID {dto.CityId} not found");

            if (dto.Temperature < -100 || dto.Temperature > 60)
                throw new BusinessException("Temperature must be between -100°C and 60°C");

            if (dto.Humidity < 0 || dto.Humidity > 100)
                throw new BusinessException("Humidity must be between 0 and 100");

            var record = new WeatherRecord
            {
                CityId = dto.CityId,
                ObservationTime = dto.ObservationTime ?? DateTime.UtcNow,
                Temperature = dto.Temperature,
                FeelsLike = dto.FeelsLike,
                Humidity = dto.Humidity,
                WindSpeed = dto.WindSpeed,
                WindDirection = dto.WindDirection,
                Pressure = dto.Pressure,
                Condition = dto.Condition,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _weatherRecordRepository.AddAsync(record);
            _logger.LogInformation("Created weather record for city {CityId}", dto.CityId);

            await CheckAndCreateAlertsAsync(city, record);

            return MapToDto(record);
        }

        //Fetch from OpenWeather 
        public async Task<WeatherRecordDto?> FetchFromOpenWeatherAsync(int cityId)
        {
            var city = await _cityRepository.GetByIdAsync(cityId);
            if (city == null)
                throw new EntityNotFoundException($"City with ID {cityId} not found");

            _logger.LogInformation("Fetching real weather data from OpenWeather for {CityName}", city.Name);

            // Call the OpenWeather service
            var weatherData = await _openWeatherClient.GetCurrentWeatherAsync(city.Latitude, city.Longitude);

            if (weatherData == null)
            {
                throw new BusinessException("Failed to fetch weather data from OpenWeather API. Please check the API key and city coordinates.");
            }

            // Convert wind degrees to direction
            string windDirection = GetWindDirection((int)weatherData.WindDegree);

            var dto = new CreateWeatherRecordDto
            {
                CityId = cityId,
                ObservationTime = DateTime.UtcNow,
                Temperature = weatherData.Temperature,
                FeelsLike = weatherData.FeelsLike,
                Humidity = weatherData.Humidity,
                WindSpeed = weatherData.WindSpeed,
                WindDirection = windDirection,
                Pressure = weatherData.Pressure,
                Condition = weatherData.Condition,
                Description = weatherData.Description
            };

            // Save to database (this will also trigger alert logic)
            var record = await CreateAsync(dto);

            _logger.LogInformation("Successfully fetched and saved real weather for {CityName}: {Temp}°C",
                city.Name, record.Temperature);

            return record;
        }

        private string GetWindDirection(int degrees)
        {
            if (degrees >= 337.5 || degrees < 22.5) return "N";
            if (degrees >= 22.5 && degrees < 67.5) return "NE";
            if (degrees >= 67.5 && degrees < 112.5) return "E";
            if (degrees >= 112.5 && degrees < 157.5) return "SE";
            if (degrees >= 157.5 && degrees < 202.5) return "S";
            if (degrees >= 202.5 && degrees < 247.5) return "SW";
            if (degrees >= 247.5 && degrees < 292.5) return "W";
            if (degrees >= 292.5 && degrees < 337.5) return "NW";
            return "N";
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _weatherRecordRepository.DeleteAsync(id);
            if (result)
                _logger.LogInformation("Deleted weather record: {RecordId}", id);
            return result;
        }

        private async Task CheckAndCreateAlertsAsync(City city, WeatherRecord record)
        {
            if (record.Temperature > 35)
            {
                var alertDto = new CreateAlertDto
                {
                    Title = $"Extreme Heat in {city.Name}",
                    Description = $"Temperature has reached {record.Temperature}°C. Stay hydrated and avoid outdoor activities.",
                    Severity = "High",
                    AlertType = "Temperature",
                    StartTime = record.ObservationTime,
                    EndTime = record.ObservationTime.AddHours(24),
                    CityIds = new List<int> { city.Id }
                };

                await _alertService.CreateAsync(alertDto);
                _logger.LogWarning("Created extreme heat alert for {CityName}", city.Name);
            }
            else if (record.Temperature < -20)
            {
                var alertDto = new CreateAlertDto
                {
                    Title = $"Extreme Cold in {city.Name}",
                    Description = $"Temperature has dropped to {record.Temperature}°C. Take precautions against frostbite.",
                    Severity = "High",
                    AlertType = "Temperature",
                    StartTime = record.ObservationTime,
                    EndTime = record.ObservationTime.AddHours(24),
                    CityIds = new List<int> { city.Id }
                };

                await _alertService.CreateAsync(alertDto);
                _logger.LogWarning("Created extreme cold alert for {CityName}", city.Name);
            }
        }

        private WeatherRecordDto MapToDto(WeatherRecord record)
        {
            return new WeatherRecordDto
            {
                Id = record.Id,
                CityId = record.CityId,
                CityName = record.City?.Name,
                ObservationTime = record.ObservationTime,
                Temperature = record.Temperature,
                FeelsLike = record.FeelsLike,
                Humidity = record.Humidity,
                WindSpeed = record.WindSpeed,
                WindDirection = record.WindDirection,
                Pressure = record.Pressure,
                Condition = record.Condition,
                Description = record.Description,
                CreatedAt = record.CreatedAt
            };
        }
    }
}