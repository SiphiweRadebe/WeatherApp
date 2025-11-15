using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherApp.Data.Entities;

namespace WeatherApp.Data.Configurations
{
    public class WeatherRecordConfiguration : IEntityTypeConfiguration<WeatherRecord>
    {
        public void Configure(EntityTypeBuilder<WeatherRecord> builder)
        {
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Temperature).HasPrecision(5, 2).IsRequired();
            builder.Property(w => w.FeelsLike).HasPrecision(5, 2);
            builder.Property(w => w.Humidity).IsRequired();
            builder.Property(w => w.WindSpeed).HasPrecision(5, 2);
            builder.Property(w => w.WindDirection).HasMaxLength(10);
            builder.Property(w => w.Pressure).HasPrecision(6, 2);
            builder.Property(w => w.Condition).HasMaxLength(50);
            builder.Property(w => w.Description).HasMaxLength(200);
            builder.Property(w => w.ObservationTime).IsRequired();
            builder.Property(w => w.CreatedAt).IsRequired();

            builder.HasIndex(w => new { w.CityId, w.ObservationTime });
        }
    }
}