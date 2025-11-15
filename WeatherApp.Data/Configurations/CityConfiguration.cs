using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherApp.Data.Entities;

namespace WeatherApp.Data.Configurations
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Country).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Latitude).HasPrecision(9, 6);
            builder.Property(c => c.Longitude).HasPrecision(9, 6);
            builder.Property(c => c.TimeZone).HasMaxLength(50);
            builder.Property(c => c.CreatedAt).IsRequired();

            builder.HasIndex(c => new { c.Name, c.Country }).IsUnique();

            builder.HasMany(c => c.WeatherRecords)
                .WithOne(w => w.City)
                .HasForeignKey(w => w.CityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}