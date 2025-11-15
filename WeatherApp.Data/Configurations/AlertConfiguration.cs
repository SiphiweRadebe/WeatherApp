using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherApp.Data.Entities;

namespace WeatherApp.Data.Configurations
{
    public class AlertConfiguration : IEntityTypeConfiguration<Alert>
    {
        public void Configure(EntityTypeBuilder<Alert> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
            builder.Property(a => a.Description).IsRequired().HasMaxLength(1000);
            builder.Property(a => a.Severity).IsRequired().HasMaxLength(20);
            builder.Property(a => a.AlertType).IsRequired().HasMaxLength(50);
            builder.Property(a => a.StartTime).IsRequired();
            builder.Property(a => a.IsActive).IsRequired();
            builder.Property(a => a.CreatedAt).IsRequired();

            builder.HasIndex(a => a.IsActive);
            builder.HasIndex(a => a.AlertType);
        }
    }
}