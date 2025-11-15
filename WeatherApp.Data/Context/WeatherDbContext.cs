using Microsoft.EntityFrameworkCore;
using WeatherApp.Data.Configurations;
using WeatherApp.Data.Entities;

namespace WeatherApp.Data.Context
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options)
        {
        }

        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<WeatherRecord> WeatherRecords { get; set; } = null!;
        public DbSet<Alert> Alerts { get; set; } = null!;
        public DbSet<CityAlert> CityAlerts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CityConfiguration());
            modelBuilder.ApplyConfiguration(new WeatherRecordConfiguration());
            modelBuilder.ApplyConfiguration(new AlertConfiguration());
            modelBuilder.ApplyConfiguration(new CityAlertConfiguration());
        }
    }
}
