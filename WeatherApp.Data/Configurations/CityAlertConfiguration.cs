using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherApp.Data.Entities;

namespace WeatherApp.Data.Configurations
{
    public class CityAlertConfiguration : IEntityTypeConfiguration<CityAlert>
    {
        public void Configure(EntityTypeBuilder<CityAlert> builder)
        {
            builder.HasKey(ca => new { ca.CityId, ca.AlertId });
            builder.Property(ca => ca.AssociatedAt).IsRequired();
            builder.Property(ca => ca.NotificationSent).IsRequired();

            builder.HasOne(ca => ca.City)
                .WithMany(c => c.CityAlerts)
                .HasForeignKey(ca => ca.CityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ca => ca.Alert)
                .WithMany(a => a.CityAlerts)
                .HasForeignKey(ca => ca.AlertId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(ca => ca.CityId);
            builder.HasIndex(ca => ca.AlertId);
        }
    }
}