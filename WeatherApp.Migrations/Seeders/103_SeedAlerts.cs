using FluentMigrator;
using System;

namespace WeatherApp.Migrations.Seeders
{
    [Migration(103)]
    public class SeedAlerts : Migration
    {
        public override void Up()
        {
            var now = DateTime.UtcNow;

            // Enable IDENTITY_INSERT for Alerts table
            Execute.Sql("SET IDENTITY_INSERT [dbo].[Alerts] ON");

            Insert.IntoTable("Alerts").Row(new
            {
                Id = 1,
                Title = "Extreme Heat Warning",
                Description = "Temperatures expected to exceed 35°C. Stay hydrated and avoid outdoor activities during peak hours.",
                Severity = "High",
                AlertType = "Temperature",
                StartTime = now,
                EndTime = now.AddDays(2),
                IsActive = true,
                CreatedAt = now
            });

            Insert.IntoTable("Alerts").Row(new
            {
                Id = 2,
                Title = "Severe Storm Alert",
                Description = "Heavy rainfall and strong winds expected. Potential for flooding in low-lying areas.",
                Severity = "Extreme",
                AlertType = "Storm",
                StartTime = now.AddHours(-1),
                EndTime = now.AddHours(12),
                IsActive = true,
                CreatedAt = now
            });

            Insert.IntoTable("Alerts").Row(new
            {
                Id = 3,
                Title = "Wind Advisory",
                Description = "Sustained winds of 40-50 km/h with gusts up to 70 km/h possible.",
                Severity = "Medium",
                AlertType = "Wind",
                StartTime = now,
                EndTime = now.AddHours(24),
                IsActive = true,
                CreatedAt = now
            });

            // Disable IDENTITY_INSERT for Alerts table
            Execute.Sql("SET IDENTITY_INSERT [dbo].[Alerts] OFF");

            // Insert CityAlerts (no IDENTITY_INSERT needed if CityAlerts doesn't have an Id column)
            Insert.IntoTable("CityAlerts").Row(new
            {
                CityId = 3,
                AlertId = 2,
                AssociatedAt = now,
                NotificationSent = true
            });

            Insert.IntoTable("CityAlerts").Row(new
            {
                CityId = 1,
                AlertId = 3,
                AssociatedAt = now,
                NotificationSent = false
            });

            Insert.IntoTable("CityAlerts").Row(new
            {
                CityId = 4,
                AlertId = 1,
                AssociatedAt = now,
                NotificationSent = true
            });
        }

        public override void Down()
        {
            Delete.FromTable("CityAlerts").AllRows();
            Delete.FromTable("Alerts").AllRows();
        }
    }
}