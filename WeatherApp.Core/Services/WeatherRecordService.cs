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

        public WeatherRecordService(
            IWeatherRecordRepository weatherRecordRepository,
            ICityRepository cityRepository,
            IAlertService alertService,
            ILogger<WeatherRecordService> logger)
        {
            _weatherRecordRepository = weatherRecordRepository;
            _cityRepository = cityRepository;
            _alertService = alertService;
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

        // NEW METHOD - Fetch from OpenWeather (mock version)
        public async Task<WeatherRecordDto?> FetchFromOpenWeatherAsync(int cityId)
        {
            var city = await _cityRepository.GetByIdAsync(cityId);
            if (city == null)
                throw new EntityNotFoundException($"City with ID {cityId} not found");

            _logger.LogInformation("Generating mock weather data for {CityName}", city.Name);

            // Create mock weather data
            var random = new Random();
            var dto = new CreateWeatherRecordDto
            {
                CityId = cityId,
                ObservationTime = DateTime.UtcNow,
                Temperature = random.Next(15, 35),
                FeelsLike = random.Next(15, 35),
                Humidity = random.Next(40, 80),
                WindSpeed = random.Next(5, 25),
                WindDirection = new[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" }[random.Next(8)],
                Pressure = random.Next(980, 1030),
                Condition = new[] { "Clear", "Sunny", "Cloudy", "Rainy", "Partly Cloudy" }[random.Next(5)],
                Description = "Mock weather data fetched from simulated API"
            };

            // Use existing CreateAsync method to save and trigger alerts
            var record = await CreateAsync(dto);

            _logger.LogInformation("Successfully created mock weather for {CityName}", city.Name);
            return record;
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