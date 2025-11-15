using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApp.Data.Context;
using WeatherApp.Data.Entities;
using WeatherApp.Data.IRepositories;

namespace WeatherApp.Data.Repositories
{
    public interface IWeatherRecordRepository : IRepository<WeatherRecord>
    {
        Task<IEnumerable<WeatherRecord>> GetByCityIdAsync(int cityId);
        Task<IEnumerable<WeatherRecord>> GetRecentByCityIdAsync(int cityId, int count = 10);
        Task<WeatherRecord?> GetLatestByCityIdAsync(int cityId);
        Task<IEnumerable<WeatherRecord>> GetByDateRangeAsync(int cityId, DateTime startDate, DateTime endDate);
    }

    public class WeatherRecordRepository : Repository<WeatherRecord>, IWeatherRecordRepository
    {
        public WeatherRecordRepository(WeatherDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<WeatherRecord>> GetByCityIdAsync(int cityId)
        {
            return await _dbSet
                .Where(w => w.CityId == cityId)
                .Include(w => w.City)
                .OrderByDescending(w => w.ObservationTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<WeatherRecord>> GetRecentByCityIdAsync(int cityId, int count = 10)
        {
            return await _dbSet
                .Where(w => w.CityId == cityId)
                .Include(w => w.City)
                .OrderByDescending(w => w.ObservationTime)
                .Take(count)
                .ToListAsync();
        }

        public async Task<WeatherRecord?> GetLatestByCityIdAsync(int cityId)
        {
            return await _dbSet
                .Where(w => w.CityId == cityId)
                .Include(w => w.City)
                .OrderByDescending(w => w.ObservationTime)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<WeatherRecord>> GetByDateRangeAsync(int cityId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(w => w.CityId == cityId &&
                           w.ObservationTime >= startDate &&
                           w.ObservationTime <= endDate)
                .Include(w => w.City)
                .OrderBy(w => w.ObservationTime)
                .ToListAsync();
        }
    }
}