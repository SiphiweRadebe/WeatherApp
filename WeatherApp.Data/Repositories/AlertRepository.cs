using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApp.Data.Context;
using WeatherApp.Data.Entities;
using WeatherApp.Data.IRepositories;

namespace WeatherApp.Data.Repositories
{
    public interface IAlertRepository : IRepository<Alert>
    {
        Task<IEnumerable<Alert>> GetActiveAlertsAsync();
        Task<IEnumerable<Alert>> GetAlertsByCityIdAsync(int cityId);
        Task<IEnumerable<Alert>> GetAlertsByTypeAsync(string alertType);
        Task<Alert?> GetAlertWithCitiesAsync(int alertId);
        Task AssociateAlertWithCityAsync(int cityId, int alertId);
        Task RemoveAlertFromCityAsync(int cityId, int alertId);
    }

    public class AlertRepository : Repository<Alert>, IAlertRepository
    {
        public AlertRepository(WeatherDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Alert>> GetActiveAlertsAsync()
        {
            return await _dbSet
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.Severity)
                .ToListAsync();
        }

        public async Task<IEnumerable<Alert>> GetAlertsByCityIdAsync(int cityId)
        {
            return await _context.CityAlerts
                .Where(ca => ca.CityId == cityId)
                .Include(ca => ca.Alert)
                .Select(ca => ca.Alert)
                .ToListAsync();
        }

        public async Task<IEnumerable<Alert>> GetAlertsByTypeAsync(string alertType)
        {
            return await _dbSet
                .Where(a => a.AlertType == alertType && a.IsActive)
                .OrderByDescending(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<Alert?> GetAlertWithCitiesAsync(int alertId)
        {
            return await _dbSet
                .Include(a => a.CityAlerts)
                    .ThenInclude(ca => ca.City)
                .FirstOrDefaultAsync(a => a.Id == alertId);
        }

        public async Task AssociateAlertWithCityAsync(int cityId, int alertId)
        {
            var cityAlert = new CityAlert
            {
                CityId = cityId,
                AlertId = alertId,
                AssociatedAt = System.DateTime.UtcNow,
                NotificationSent = false
            };

            _context.CityAlerts.Add(cityAlert);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAlertFromCityAsync(int cityId, int alertId)
        {
            var cityAlert = await _context.CityAlerts
                .FirstOrDefaultAsync(ca => ca.CityId == cityId && ca.AlertId == alertId);

            if (cityAlert != null)
            {
                _context.CityAlerts.Remove(cityAlert);
                await _context.SaveChangesAsync();
            }
        }
    }
}