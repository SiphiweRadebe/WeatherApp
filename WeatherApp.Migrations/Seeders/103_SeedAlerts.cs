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

            var alerts = new[]
            {
                new { Id = 1, Title = "Extreme Heat Warning", Description = "Temperatures expected to exceed 35°C. Stay hydrated and avoid outdoor activities.", Severity = "High", AlertType = "Temperature", StartTime = now, EndTime = now.AddDays(2), IsActive = true },
                new { Id = 2, Title = "Severe Storm Alert", Description = "Heavy rainfall and strong winds expected. Potential flooding in low-lying areas.", Severity = "Extreme", AlertType = "Storm", StartTime = now.AddHours(-1), EndTime = now.AddHours(12), IsActive = true },
                new { Id = 3, Title = "Wind Advisory", Description = "Sustained winds of 40-50 km/h with gusts up to 70 km/h possible.", Severity = "Medium", AlertType = "Wind", StartTime = now, EndTime = now.AddHours(24), IsActive = true },
                new { Id = 4, Title = "Flood Warning", Description = "Heavy rainfall may cause localized flooding. Avoid driving through flooded areas.", Severity = "High", AlertType = "Flood", StartTime = now, EndTime = now.AddDays(1), IsActive = true },
                new { Id = 5, Title = "Snow Advisory", Description = "Snow expected in the area, with icy conditions on roads.", Severity = "Medium", AlertType = "Snow", StartTime = now, EndTime = now.AddDays(1), IsActive = true }
            };

            foreach (var alert in alerts)
            {
                Insert.IntoTable("Alerts").Row(new
                {
                    alert.Id,
                    alert.Title,
                    alert.Description,
                    alert.Severity,
                    alert.AlertType,
                    alert.StartTime,
                    alert.EndTime,
                    alert.IsActive,
                    CreatedAt = now
                });
            }

            Execute.Sql("SET IDENTITY_INSERT [dbo].[Alerts] OFF");

            // Associate alerts with cities (CityAlerts)
            var cityAlerts = new[]
            {
                new { CityId = 1, AlertId = 1, AssociatedAt = now, NotificationSent = true },
                new { CityId = 2, AlertId = 2, AssociatedAt = now, NotificationSent = false },
                new { CityId = 3, AlertId = 2, AssociatedAt = now, NotificationSent = true },
                new { CityId = 4, AlertId = 3, AssociatedAt = now, NotificationSent = false },
                new { CityId = 5, AlertId = 1, AssociatedAt = now, NotificationSent = true },
                new { CityId = 6, AlertId = 1, AssociatedAt = now, NotificationSent = true },
                new { CityId = 7, AlertId = 3, AssociatedAt = now, NotificationSent = false },
                new { CityId = 8, AlertId = 1, AssociatedAt = now, NotificationSent = true },
                new { CityId = 9, AlertId = 1, AssociatedAt = now, NotificationSent = true },
                new { CityId = 10, AlertId = 2, AssociatedAt = now, NotificationSent = false },
                new { CityId = 11, AlertId = 2, AssociatedAt = now, NotificationSent = true },
                new { CityId = 12, AlertId = 3, AssociatedAt = now, NotificationSent = false },
                new { CityId = 13, AlertId = 2, AssociatedAt = now, NotificationSent = true },
                new { CityId = 14, AlertId = 4, AssociatedAt = now, NotificationSent = false },
                new { CityId = 15, AlertId = 4, AssociatedAt = now, NotificationSent = true },
                new { CityId = 16, AlertId = 2, AssociatedAt = now, NotificationSent = true },
                new { CityId = 17, AlertId = 1, AssociatedAt = now, NotificationSent = false },
                new { CityId = 18, AlertId = 1, AssociatedAt = now, NotificationSent = true },
                new { CityId = 19, AlertId = 1, AssociatedAt = now, NotificationSent = true },
                new { CityId = 20, AlertId = 3, AssociatedAt = now, NotificationSent = false },
                new { CityId = 21, AlertId = 2, AssociatedAt = now, NotificationSent = true },
                new { CityId = 22, AlertId = 3, AssociatedAt = now, NotificationSent = false },
                new { CityId = 23, AlertId = 5, AssociatedAt = now, NotificationSent = true },
                new { CityId = 24, AlertId = 2, AssociatedAt = now, NotificationSent = false },
                new { CityId = 25, AlertId = 1, AssociatedAt = now, NotificationSent = true },
                new { CityId = 26, AlertId = 2, AssociatedAt = now, NotificationSent = true },
                new { CityId = 27, AlertId = 3, AssociatedAt = now, NotificationSent = false },
                new { CityId = 28, AlertId = 5, AssociatedAt = now, NotificationSent = true },
                new { CityId = 29, AlertId = 5, AssociatedAt = now, NotificationSent = true },
                new { CityId = 30, AlertId = 3, AssociatedAt = now, NotificationSent = false }
            };

            foreach (var ca in cityAlerts)
            {
                Insert.IntoTable("CityAlerts").Row(new
                {
                    ca.CityId,
                    ca.AlertId,
                    ca.AssociatedAt,
                    ca.NotificationSent
                });
            }
        }

        public override void Down()
        {
            Delete.FromTable("CityAlerts").AllRows();
            Delete.FromTable("Alerts").AllRows();
        }
    }
}
