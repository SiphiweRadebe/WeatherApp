using FluentMigrator;
using System;
using System.Collections.Generic;

namespace WeatherApp.Migrations.Seeders
{
    [Migration(101)]
    public class SeedCities : Migration
    {
        public override void Up()
        {
            var now = DateTime.UtcNow;

            var cities = new List<dynamic>
            {
                new { Id = 1, Name = "London", Country = "United Kingdom", Latitude = 51.5074m, Longitude = -0.1278m, TimeZone = "Europe/London" },
                new { Id = 2, Name = "New York", Country = "United States", Latitude = 40.7128m, Longitude = -74.0060m, TimeZone = "America/New_York" },
                new { Id = 3, Name = "Tokyo", Country = "Japan", Latitude = 35.6762m, Longitude = 139.6503m, TimeZone = "Asia/Tokyo" },
                new { Id = 4, Name = "Sydney", Country = "Australia", Latitude = -33.8688m, Longitude = 151.2093m, TimeZone = "Australia/Sydney" },
                new { Id = 5, Name = "Paris", Country = "France", Latitude = 48.8566m, Longitude = 2.3522m, TimeZone = "Europe/Paris" },

                new { Id = 6,  Name = "Berlin", Country = "Germany", Latitude = 52.5200m, Longitude = 13.4050m, TimeZone = "Europe/Berlin" },
                new { Id = 7,  Name = "Madrid", Country = "Spain", Latitude = 40.4168m, Longitude = -3.7038m, TimeZone = "Europe/Madrid" },
                new { Id = 8,  Name = "Rome", Country = "Italy", Latitude = 41.9028m, Longitude = 12.4964m, TimeZone = "Europe/Rome" },
                new { Id = 9,  Name = "Toronto", Country = "Canada", Latitude = 43.6532m, Longitude = -79.3832m, TimeZone = "America/Toronto" },
                new { Id = 10, Name = "Mexico City", Country = "Mexico", Latitude = 19.4326m, Longitude = -99.1332m, TimeZone = "America/Mexico_City" },

                new { Id = 11, Name = "São Paulo", Country = "Brazil", Latitude = -23.5505m, Longitude = -46.6333m, TimeZone = "America/Sao_Paulo" },
                new { Id = 12, Name = "Buenos Aires", Country = "Argentina", Latitude = -34.6037m, Longitude = -58.3816m, TimeZone = "America/Argentina/Buenos_Aires" },
                new { Id = 13, Name = "Cape Town", Country = "South Africa", Latitude = -33.9249m, Longitude = 18.4241m, TimeZone = "Africa/Johannesburg" },
                new { Id = 14, Name = "Johannesburg", Country = "South Africa", Latitude = -26.2041m, Longitude = 28.0473m, TimeZone = "Africa/Johannesburg" },
                new { Id = 15, Name = "Nairobi", Country = "Kenya", Latitude = -1.2921m, Longitude = 36.8219m, TimeZone = "Africa/Nairobi" },

                new { Id = 16, Name = "Dubai", Country = "United Arab Emirates", Latitude = 25.2048m, Longitude = 55.2708m, TimeZone = "Asia/Dubai" },
                new { Id = 17, Name = "Mumbai", Country = "India", Latitude = 19.0760m, Longitude = 72.8777m, TimeZone = "Asia/Kolkata" },
                new { Id = 18, Name = "Singapore", Country = "Singapore", Latitude = 1.3521m, Longitude = 103.8198m, TimeZone = "Asia/Singapore" },
                new { Id = 19, Name = "Hong Kong", Country = "China", Latitude = 22.3193m, Longitude = 114.1694m, TimeZone = "Asia/Hong_Kong" },
                new { Id = 20, Name = "Seoul", Country = "South Korea", Latitude = 37.5665m, Longitude = 126.9780m, TimeZone = "Asia/Seoul" },

                new { Id = 21, Name = "Los Angeles", Country = "United States", Latitude = 34.0522m, Longitude = -118.2437m, TimeZone = "America/Los_Angeles" },
                new { Id = 22, Name = "Chicago", Country = "United States", Latitude = 41.8781m, Longitude = -87.6298m, TimeZone = "America/Chicago" },
                new { Id = 23, Name = "Moscow", Country = "Russia", Latitude = 55.7558m, Longitude = 37.6173m, TimeZone = "Europe/Moscow" },
                new { Id = 24, Name = "Istanbul", Country = "Turkey", Latitude = 41.0082m, Longitude = 28.9784m, TimeZone = "Europe/Istanbul" },
                new { Id = 25, Name = "Cairo", Country = "Egypt", Latitude = 30.0444m, Longitude = 31.2357m, TimeZone = "Africa/Cairo" },

                new { Id = 26, Name = "Bangkok", Country = "Thailand", Latitude = 13.7563m, Longitude = 100.5018m, TimeZone = "Asia/Bangkok" },
                new { Id = 27, Name = "Jakarta", Country = "Indonesia", Latitude = -6.2088m, Longitude = 106.8456m, TimeZone = "Asia/Jakarta" },
                new { Id = 28, Name = "Lagos", Country = "Nigeria", Latitude = 6.5244m, Longitude = 3.3792m, TimeZone = "Africa/Lagos" },
                new { Id = 29, Name = "Beijing", Country = "China", Latitude = 39.9042m, Longitude = 116.4074m, TimeZone = "Asia/Shanghai" },
                new { Id = 30, Name = "San Francisco", Country = "United States", Latitude = 37.7749m, Longitude = -122.4194m, TimeZone = "America/Los_Angeles" },
            };

            Execute.Sql("SET IDENTITY_INSERT [dbo].[Cities] ON");

            foreach (var city in cities)
            {
                Insert.IntoTable("Cities").Row(new
                {
                    city.Id,
                    city.Name,
                    city.Country,
                    city.Latitude,
                    city.Longitude,
                    city.TimeZone,
                    CreatedAt = now
                });
            }

            Execute.Sql("SET IDENTITY_INSERT [dbo].[Cities] OFF");
        }

        public override void Down()
        {
            for (int id = 1; id <= 30; id++)
            {
                Delete.FromTable("Cities").Row(new { Id = id });
            }
        }
    }
}
