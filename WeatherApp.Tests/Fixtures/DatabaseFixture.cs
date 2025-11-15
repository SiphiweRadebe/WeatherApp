using Microsoft.EntityFrameworkCore;
using WeatherApp.Data.Context;
using WeatherApp.Data.Entities;

namespace WeatherApp.Tests.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        public WeatherDbContext Context { get; private set; }

        public DatabaseFixture()
        {
            var options = new DbContextOptionsBuilder<WeatherDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Context = new WeatherDbContext(options);
            SeedTestData();
        }

        private void SeedTestData()
        {
            var cities = new[]
            {
                new City
                {
                    Id = 1,
                    Name = "TestCity1",
                    Country = "TestCountry1",
                    Latitude = 51.5074m,
                    Longitude = -0.1278m,
                    TimeZone = "Europe/London",
                    CreatedAt = DateTime.UtcNow
                },
                new City
                {
                    Id = 2,
                    Name = "TestCity2",
                    Country = "TestCountry2",
                    Latitude = 40.7128m,
                    Longitude = -74.0060m,
                    TimeZone = "America/New_York",
                    CreatedAt = DateTime.UtcNow
                }
            };

            Context.Cities.AddRange(cities);

            var weatherRecords = new[]
            {
                new WeatherRecord
                {
                    Id = 1,
                    CityId = 1,
                    ObservationTime = DateTime.UtcNow.AddHours(-1),
                    Temperature = 20.5m,
                    FeelsLike = 19.0m,
                    Humidity = 65,
                    WindSpeed = 10.5m,
                    WindDirection = "NW",
                    Pressure = 1013.25m,
                    Condition = "Cloudy",
                    Description = "Test weather record",
                    CreatedAt = DateTime.UtcNow
                }
            };

            Context.WeatherRecords.AddRange(weatherRecords);

            var alerts = new[]
            {
                new Alert
                {
                    Id = 1,
                    Title = "Test Alert",
                    Description = "Test Description",
                    Severity = "High",
                    AlertType = "Temperature",
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow.AddDays(1),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            Context.Alerts.AddRange(alerts);
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}