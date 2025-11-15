using FluentMigrator;
using System;

namespace WeatherApp.Migrations.Seeders
{
    [Migration(101)]
    public class SeedCities : Migration
    {
        public override void Up()
        {
            var now = DateTime.UtcNow;

            Insert.IntoTable("Cities").Row(new
            {
                Id = 1,
                Name = "London",
                Country = "United Kingdom",
                Latitude = 51.5074m,
                Longitude = -0.1278m,
                TimeZone = "Europe/London",
                CreatedAt = now
            });

            Insert.IntoTable("Cities").Row(new
            {
                Id = 2,
                Name = "New York",
                Country = "United States",
                Latitude = 40.7128m,
                Longitude = -74.0060m,
                TimeZone = "America/New_York",
                CreatedAt = now
            });

            Insert.IntoTable("Cities").Row(new
            {
                Id = 3,
                Name = "Tokyo",
                Country = "Japan",
                Latitude = 35.6762m,
                Longitude = 139.6503m,
                TimeZone = "Asia/Tokyo",
                CreatedAt = now
            });

            Insert.IntoTable("Cities").Row(new
            {
                Id = 4,
                Name = "Sydney",
                Country = "Australia",
                Latitude = -33.8688m,
                Longitude = 151.2093m,
                TimeZone = "Australia/Sydney",
                CreatedAt = now
            });

            Insert.IntoTable("Cities").Row(new
            {
                Id = 5,
                Name = "Paris",
                Country = "France",
                Latitude = 48.8566m,
                Longitude = 2.3522m,
                TimeZone = "Europe/Paris",
                CreatedAt = now
            });
        }

        public override void Down()
        {
            Delete.FromTable("Cities").Row(new { Id = 1 });
            Delete.FromTable("Cities").Row(new { Id = 2 });
            Delete.FromTable("Cities").Row(new { Id = 3 });
            Delete.FromTable("Cities").Row(new { Id = 4 });
            Delete.FromTable("Cities").Row(new { Id = 5 });
        }
    }
}