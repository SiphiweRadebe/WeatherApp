using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApp.Data.Context;
using WeatherApp.Data.Entities;
using WeatherApp.Data.IRepositories;

namespace WeatherApp.Data.Repositories
{
    public interface ICityRepository : IRepository<City>
    {
        Task<City?> GetByNameAndCountryAsync(string name, string country);
        Task<IEnumerable<City>> GetCitiesWithWeatherRecordsAsync();
        Task<City?> GetCityWithAlertsAsync(int cityId);
    }

    public class CityRepository : Repository<City>, ICityRepository
    {
        public CityRepository(WeatherDbContext context) : base(context)
        {
        }

        public async Task<City?> GetByNameAndCountryAsync(string name, string country)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Name == name && c.Country == country);
        }

        public async Task<IEnumerable<City>> GetCitiesWithWeatherRecordsAsync()
        {
            return await _dbSet
                .Include(c => c.WeatherRecords)
                .ToListAsync();
        }

        public async Task<City?> GetCityWithAlertsAsync(int cityId)
        {
            return await _dbSet
                .Include(c => c.CityAlerts)
                    .ThenInclude(ca => ca.Alert)
                .FirstOrDefaultAsync(c => c.Id == cityId);
        }
    }
}