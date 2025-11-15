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
    public class AlertService : IAlertService
    {
        private readonly IAlertRepository _alertRepository;
        private readonly ICityRepository _cityRepository;
        private readonly ILogger<AlertService> _logger;

        public AlertService(
            IAlertRepository alertRepository,
            ICityRepository cityRepository,
            ILogger<AlertService> logger)
        {
            _alertRepository = alertRepository;
            _cityRepository = cityRepository;
            _logger = logger;
        }

        public async Task<AlertDto> GetByIdAsync(int id)
        {
            var alert = await _alertRepository.GetAlertWithCitiesAsync(id);
            if (alert == null)
                throw new EntityNotFoundException($"Alert with ID {id} not found");

            return MapToDto(alert);
        }

        public async Task<IEnumerable<AlertDto>> GetAllActiveAsync()
        {
            var alerts = await _alertRepository.GetActiveAlertsAsync();
            return alerts.Select(a => MapToDto(a));
        }

        public async Task<IEnumerable<AlertDto>> GetByCityIdAsync(int cityId)
        {
            var alerts = await _alertRepository.GetAlertsByCityIdAsync(cityId);
            return alerts.Select(a => MapToDto(a));
        }

        public async Task<AlertDto> CreateAsync(CreateAlertDto dto)
        {
            var validSeverities = new[] { "Low", "Medium", "High", "Extreme" };
            if (!validSeverities.Contains(dto.Severity))
                throw new BusinessException($"Severity must be one of: {string.Join(", ", validSeverities)}");

            var validTypes = new[] { "Temperature", "Storm", "Wind", "Flood", "Snow", "Other" };
            if (!validTypes.Contains(dto.AlertType))
                throw new BusinessException($"Alert type must be one of: {string.Join(", ", validTypes)}");

            var alert = new Alert
            {
                Title = dto.Title,
                Description = dto.Description,
                Severity = dto.Severity,
                AlertType = dto.AlertType,
                StartTime = dto.StartTime ?? DateTime.UtcNow,
                EndTime = dto.EndTime,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _alertRepository.AddAsync(alert);
            _logger.LogInformation("Created alert: {AlertTitle}", alert.Title);

            if (dto.CityIds != null && dto.CityIds.Any())
            {
                foreach (var cityId in dto.CityIds)
                {
                    var city = await _cityRepository.GetByIdAsync(cityId);
                    if (city == null)
                    {
                        _logger.LogWarning("City with ID {CityId} not found, skipping association", cityId);
                        continue;
                    }

                    await _alertRepository.AssociateAlertWithCityAsync(cityId, alert.Id);
                }
            }

            return MapToDto(alert);
        }

        public async Task<AlertDto> UpdateAsync(int id, UpdateAlertDto dto)
        {
            var alert = await _alertRepository.GetByIdAsync(id);
            if (alert == null)
                throw new EntityNotFoundException($"Alert with ID {id} not found");

            if (!string.IsNullOrWhiteSpace(dto.Title))
                alert.Title = dto.Title;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                alert.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.Severity))
            {
                var validSeverities = new[] { "Low", "Medium", "High", "Extreme" };
                if (!validSeverities.Contains(dto.Severity))
                    throw new BusinessException($"Severity must be one of: {string.Join(", ", validSeverities)}");
                alert.Severity = dto.Severity;
            }

            if (dto.EndTime.HasValue)
                alert.EndTime = dto.EndTime.Value;

            if (dto.IsActive.HasValue)
                alert.IsActive = dto.IsActive.Value;

            alert.UpdatedAt = DateTime.UtcNow;

            await _alertRepository.UpdateAsync(alert);
            _logger.LogInformation("Updated alert: {AlertId}", id);

            return MapToDto(alert);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _alertRepository.DeleteAsync(id);
            if (result)
                _logger.LogInformation("Deleted alert: {AlertId}", id);
            return result;
        }

        private AlertDto MapToDto(Alert alert)
        {
            return new AlertDto
            {
                Id = alert.Id,
                Title = alert.Title,
                Description = alert.Description,
                Severity = alert.Severity,
                AlertType = alert.AlertType,
                StartTime = alert.StartTime,
                EndTime = alert.EndTime,
                IsActive = alert.IsActive,
                CreatedAt = alert.CreatedAt,
                AffectedCities = alert.CityAlerts?
                    .Select(ca => ca.City.Name)
                    .ToList()
            };
        }
    }
}