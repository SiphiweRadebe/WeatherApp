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

            Execute.Sql("SET IDENTITY_INSERT [dbo].[WeatherRecords] ON");

            var weatherRecords = new[]
            {
                new { Id = 1, CityId = 1, ObservationTime = now.AddHours(-2), Temperature = 15.5m, FeelsLike = 14.2m, Humidity = 72, WindSpeed = 12, WindDirection = "SW", Pressure = 1013m, Condition = "Cloudy", Description = "Overcast with occasional rain" },
                new { Id = 2, CityId = 2, ObservationTime = now.AddHours(-1), Temperature = 22.3m, FeelsLike = 23.1m, Humidity = 65, WindSpeed = 8, WindDirection = "NE", Pressure = 1015m, Condition = "Sunny", Description = "Clear skies with gentle breeze" },
                new { Id = 3, CityId = 3, ObservationTime = now.AddHours(-3), Temperature = 28.7m, FeelsLike = 31.2m, Humidity = 78, WindSpeed = 15, WindDirection = "E", Pressure = 1009m, Condition = "Rainy", Description = "Heavy rain with strong winds" },
                new { Id = 4, CityId = 4, ObservationTime = now.AddHours(-4), Temperature = 18.9m, FeelsLike = 17.5m, Humidity = 55, WindSpeed = 10, WindDirection = "S", Pressure = 1018m, Condition = "Partly Cloudy", Description = "Mild with scattered clouds" },
                new { Id = 5, CityId = 5, ObservationTime = now.AddHours(-1), Temperature = 16.2m, FeelsLike = 15.8m, Humidity = 68, WindSpeed = 9, WindDirection = "W", Pressure = 1013m, Condition = "Clear", Description = "Beautiful clear evening" },
                new { Id = 6, CityId = 6, ObservationTime = now.AddHours(-2), Temperature = 20.5m, FeelsLike = 20.0m, Humidity = 60, WindSpeed = 11, WindDirection = "NE", Pressure = 1012m, Condition = "Sunny", Description = "Warm and sunny" },
                new { Id = 7, CityId = 7, ObservationTime = now.AddHours(-3), Temperature = 17.0m, FeelsLike = 16.5m, Humidity = 65, WindSpeed = 8, WindDirection = "SE", Pressure = 1014m, Condition = "Cloudy", Description = "Clouds moving across the sky" },
                new { Id = 8, CityId = 8, ObservationTime = now.AddHours(-1), Temperature = 34.2m, FeelsLike = 36.0m, Humidity = 20, WindSpeed = 6, WindDirection = "E", Pressure = 1007m, Condition = "Sunny", Description = "Hot and dry" },
                new { Id = 9, CityId = 9, ObservationTime = now.AddHours(-2), Temperature = 38.5m, FeelsLike = 41.0m, Humidity = 25, WindSpeed = 12, WindDirection = "NW", Pressure = 1005m, Condition = "Sunny", Description = "Extremely hot and sunny" },
                new { Id = 10, CityId = 10, ObservationTime = now.AddHours(-3), Temperature = 30.0m, FeelsLike = 32.0m, Humidity = 80, WindSpeed = 10, WindDirection = "SW", Pressure = 1010m, Condition = "Humid", Description = "Hot and humid with occasional showers" },
                new { Id = 11, CityId = 11, ObservationTime = now.AddHours(-2), Temperature = 10.5m, FeelsLike = 9.0m, Humidity = 60, WindSpeed = 13, WindDirection = "N", Pressure = 1016m, Condition = "Cloudy", Description = "Cool and cloudy" },
                new { Id = 12, CityId = 12, ObservationTime = now.AddHours(-1), Temperature = 25.0m, FeelsLike = 25.5m, Humidity = 50, WindSpeed = 7, WindDirection = "NE", Pressure = 1013m, Condition = "Sunny", Description = "Warm and clear" },
                new { Id = 13, CityId = 13, ObservationTime = now.AddHours(-3), Temperature = 21.0m, FeelsLike = 21.0m, Humidity = 70, WindSpeed = 5, WindDirection = "E", Pressure = 1012m, Condition = "Rainy", Description = "Light rain expected" },
                new { Id = 14, CityId = 14, ObservationTime = now.AddHours(-2), Temperature = 23.5m, FeelsLike = 23.5m, Humidity = 75, WindSpeed = 9, WindDirection = "SE", Pressure = 1011m, Condition = "Cloudy", Description = "Overcast with humidity" },
                new { Id = 15, CityId = 15, ObservationTime = now.AddHours(-1), Temperature = 18.0m, FeelsLike = 17.5m, Humidity = 60, WindSpeed = 8, WindDirection = "S", Pressure = 1015m, Condition = "Clear", Description = "Cool and clear" },
                new { Id = 16, CityId = 16, ObservationTime = now.AddHours(-2), Temperature = 26.0m, FeelsLike = 26.0m, Humidity = 70, WindSpeed = 12, WindDirection = "SW", Pressure = 1010m, Condition = "Rainy", Description = "Afternoon showers" },
                new { Id = 17, CityId = 17, ObservationTime = now.AddHours(-3), Temperature = 32.0m, FeelsLike = 34.0m, Humidity = 60, WindSpeed = 15, WindDirection = "NW", Pressure = 1009m, Condition = "Sunny", Description = "Hot and sunny" },
                new { Id = 18, CityId = 18, ObservationTime = now.AddHours(-1), Temperature = 30.5m, FeelsLike = 33.0m, Humidity = 70, WindSpeed = 10, WindDirection = "NE", Pressure = 1012m, Condition = "Humid", Description = "Hot with high humidity" },
                new { Id = 19, CityId = 19, ObservationTime = now.AddHours(-2), Temperature = 35.0m, FeelsLike = 38.0m, Humidity = 40, WindSpeed = 12, WindDirection = "E", Pressure = 1008m, Condition = "Sunny", Description = "Extremely hot" },
                new { Id = 20, CityId = 20, ObservationTime = now.AddHours(-3), Temperature = 16.0m, FeelsLike = 16.0m, Humidity = 60, WindSpeed = 10, WindDirection = "N", Pressure = 1015m, Condition = "Cloudy", Description = "Cool and cloudy" },
                new { Id = 21, CityId = 21, ObservationTime = now.AddHours(-2), Temperature = 22.0m, FeelsLike = 22.5m, Humidity = 75, WindSpeed = 8, WindDirection = "SE", Pressure = 1011m, Condition = "Rainy", Description = "Afternoon showers" },
                new { Id = 22, CityId = 22, ObservationTime = now.AddHours(-1), Temperature = 18.5m, FeelsLike = 18.5m, Humidity = 65, WindSpeed = 6, WindDirection = "SW", Pressure = 1014m, Condition = "Cloudy", Description = "Light clouds" },
                new { Id = 23, CityId = 23, ObservationTime = now.AddHours(-2), Temperature = -5.0m, FeelsLike = -10.0m, Humidity = 80, WindSpeed = 15, WindDirection = "N", Pressure = 1020m, Condition = "Snow", Description = "Cold and snowy" },
                new { Id = 24, CityId = 24, ObservationTime = now.AddHours(-1), Temperature = 10.0m, FeelsLike = 9.0m, Humidity = 70, WindSpeed = 12, WindDirection = "NE", Pressure = 1015m, Condition = "Rainy", Description = "Light rain showers" },
                new { Id = 25, CityId = 25, ObservationTime = now.AddHours(-3), Temperature = 15.0m, FeelsLike = 14.0m, Humidity = 65, WindSpeed = 8, WindDirection = "SW", Pressure = 1013m, Condition = "Sunny", Description = "Warm and sunny" },
                new { Id = 26, CityId = 26, ObservationTime = now.AddHours(-2), Temperature = 20.0m, FeelsLike = 19.5m, Humidity = 60, WindSpeed = 10, WindDirection = "S", Pressure = 1012m, Condition = "Cloudy", Description = "Overcast but mild" },
                new { Id = 27, CityId = 27, ObservationTime = now.AddHours(-1), Temperature = 12.0m, FeelsLike = 11.0m, Humidity = 75, WindSpeed = 7, WindDirection = "NW", Pressure = 1016m, Condition = "Rainy", Description = "Light rain expected" },
                new { Id = 28, CityId = 28, ObservationTime = now.AddHours(-3), Temperature = -2.0m, FeelsLike = -5.0m, Humidity = 85, WindSpeed = 12, WindDirection = "N", Pressure = 1021m, Condition = "Snowy", Description = "Cold with snow showers" },
                new { Id = 29, CityId = 29, ObservationTime = now.AddHours(-2), Temperature = -1.0m, FeelsLike = -3.0m, Humidity = 80, WindSpeed = 10, WindDirection = "NE", Pressure = 1020m, Condition = "Snow", Description = "Cold with light snow" },
                new { Id = 30, CityId = 30, ObservationTime = now.AddHours(-1), Temperature = 5.0m, FeelsLike = 4.0m, Humidity = 70, WindSpeed = 8, WindDirection = "NW", Pressure = 1018m, Condition = "Cloudy", Description = "Cool and cloudy" }
            };

            foreach (var record in weatherRecords)
            {
                Insert.IntoTable("WeatherRecords").Row(new
                {
                    record.Id,
                    record.CityId,
                    record.ObservationTime,
                    record.Temperature,
                    record.FeelsLike,
                    record.Humidity,
                    record.WindSpeed,
                    record.WindDirection,
                    record.Pressure,
                    record.Condition,
                    record.Description,
                    CreatedAt = now
                });
            }

            Execute.Sql("SET IDENTITY_INSERT [dbo].[WeatherRecords] OFF");
        }

        public override void Down()
        {
            Delete.FromTable("WeatherRecords").AllRows();
        }
    }
}
