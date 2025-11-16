using FluentMigrator;
using System;

namespace WeatherApp.Migrations.Seeders
{
    [Migration(102)]
    public class SeedWeatherRecords : Migration
    {
        public override void Up()
        {
            var now = DateTime.UtcNow;

            // Enable IDENTITY_INSERT to allow explicit ID values
            Execute.Sql("SET IDENTITY_INSERT [dbo].[WeatherRecords] ON");

            Insert.IntoTable("WeatherRecords").Row(new
            {
                Id = 1,  // ← ADD THIS
                CityId = 1,
                ObservationTime = now.AddHours(-2),
                Temperature = 15.5m,
                FeelsLike = 14.2m,
                Humidity = 72,
                WindSpeed = 12.5m,
                WindDirection = "SW",
                Pressure = 1013.25m,
                Condition = "Cloudy",
                Description = "Overcast with occasional rain",
                CreatedAt = now
            });

            Insert.IntoTable("WeatherRecords").Row(new
            {
                Id = 2,  // ← ADD THIS
                CityId = 2,
                ObservationTime = now.AddHours(-1),
                Temperature = 22.3m,
                FeelsLike = 23.1m,
                Humidity = 65,
                WindSpeed = 8.2m,
                WindDirection = "NE",
                Pressure = 1015.50m,
                Condition = "Sunny",
                Description = "Clear skies with gentle breeze",
                CreatedAt = now
            });

            Insert.IntoTable("WeatherRecords").Row(new
            {
                Id = 3,  // ← ADD THIS
                CityId = 3,
                ObservationTime = now.AddHours(-3),
                Temperature = 28.7m,
                FeelsLike = 31.2m,
                Humidity = 78,
                WindSpeed = 15.3m,
                WindDirection = "E",
                Pressure = 1008.75m,
                Condition = "Rainy",
                Description = "Heavy rain with strong winds",
                CreatedAt = now
            });

            Insert.IntoTable("WeatherRecords").Row(new
            {
                Id = 4,  // ← ADD THIS
                CityId = 4,
                ObservationTime = now.AddHours(-4),
                Temperature = 18.9m,
                FeelsLike = 17.5m,
                Humidity = 55,
                WindSpeed = 10.1m,
                WindDirection = "S",
                Pressure = 1018.20m,
                Condition = "Partly Cloudy",
                Description = "Mild with scattered clouds",
                CreatedAt = now
            });

            Insert.IntoTable("WeatherRecords").Row(new
            {
                Id = 5,  // ← ADD THIS
                CityId = 5,
                ObservationTime = now.AddHours(-1),
                Temperature = 16.2m,
                FeelsLike = 15.8m,
                Humidity = 68,
                WindSpeed = 9.7m,
                WindDirection = "W",
                Pressure = 1012.80m,
                Condition = "Clear",
                Description = "Beautiful clear evening",
                CreatedAt = now
            });

            // Disable IDENTITY_INSERT after inserts
            Execute.Sql("SET IDENTITY_INSERT [dbo].[WeatherRecords] OFF");
        }

        public override void Down()
        {
            Delete.FromTable("WeatherRecords").AllRows();
        }
    }
}