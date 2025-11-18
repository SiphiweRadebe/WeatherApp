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
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly ILogger<CityService> _logger;

        public CityService(ICityRepository cityRepository, ILogger<CityService> logger)
        {
            _cityRepository = cityRepository;
            _logger = logger;

        }

        public async Task<CityDto> GetByIdAsync(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null)
                throw new EntityNotFoundException($"City with ID {id} not found");

            return MapToDto(city);
        }

        public async Task<IEnumerable<CityDto>> GetAllAsync()
        {
            var cities = await _cityRepository.GetAllAsync();
            return cities.Select(MapToDto);
        }

        public async Task<CityDto> CreateAsync(CreateCityDto dto)
        {
            var existing = await _cityRepository.GetByNameAndCountryAsync(dto.Name, dto.Country);
            if (existing != null)
                throw new DuplicateEntityException($"City '{dto.Name}, {dto.Country}' already exists");

            if (dto.Latitude < -90 || dto.Latitude > 90)
                throw new BusinessException("Latitude must be between -90 and 90");

            if (dto.Longitude < -180 || dto.Longitude > 180)
                throw new BusinessException("Longitude must be between -180 and 180");

            var city = new City
            {
                Name = dto.Name,
                Country = dto.Country,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                TimeZone = dto.TimeZone,
                CreatedAt = DateTime.UtcNow
            };

            await _cityRepository.AddAsync(city);
            _logger.LogInformation("Created new city: {CityName}, {Country}", city.Name, city.Country);

            return MapToDto(city);
        }

        public async Task<CityDto> UpdateAsync(int id, UpdateCityDto dto)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null)
                throw new EntityNotFoundException($"City with ID {id} not found");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                city.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Country))
                city.Country = dto.Country;

            if (dto.Latitude.HasValue)
            {
                if (dto.Latitude < -90 || dto.Latitude > 90)
                    throw new BusinessException("Latitude must be between -90 and 90");
                city.Latitude = dto.Latitude.Value;
            }

            if (dto.Longitude.HasValue)
            {
                if (dto.Longitude < -180 || dto.Longitude > 180)
                    throw new BusinessException("Longitude must be between -180 and 180");
                city.Longitude = dto.Longitude.Value;
            }

            if (dto.TimeZone != null)
                city.TimeZone = dto.TimeZone;

            city.UpdatedAt = DateTime.UtcNow;

            await _cityRepository.UpdateAsync(city);
            _logger.LogInformation("Updated city: {CityId}", id);

            return MapToDto(city);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _cityRepository.DeleteAsync(id);
            if (result)
                _logger.LogInformation("Deleted city: {CityId}", id);
            return result;
        }

        public async Task<CityDto> GetCityWithWeatherAsync(int id)
        {
            var cities = await _cityRepository.GetCitiesWithWeatherRecordsAsync();
            var city = cities.FirstOrDefault(c => c.Id == id);

            if (city == null)
                throw new EntityNotFoundException($"City with ID {id} not found");

            return MapToDtoWithWeather(city);
        }

        private CityDto MapToDto(City city)
        {
            return new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                Country = city.Country,
                Latitude = city.Latitude,
                Longitude = city.Longitude,
                TimeZone = city.TimeZone,
                CreatedAt = city.CreatedAt,
                UpdatedAt = city.UpdatedAt
            };
        }

        private CityDto MapToDtoWithWeather(City city)
        {
            var dto = MapToDto(city);
            dto.WeatherRecords = city.WeatherRecords.Select(w => new WeatherRecordDto
            {
                Id = w.Id,
                CityId = w.CityId,
                ObservationTime = w.ObservationTime,
                Temperature = w.Temperature,
                FeelsLike = w.FeelsLike,
                Humidity = w.Humidity,
                WindSpeed = w.WindSpeed,
                WindDirection = w.WindDirection,
                Pressure = w.Pressure,
                Condition = w.Condition,
                Description = w.Description,
                CreatedAt = w.CreatedAt
            }).ToList();
            return dto;
        }
    }
}