using System;
using System.Collections.Generic;

namespace WeatherApp.Data.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? TimeZone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<WeatherRecord> WeatherRecords { get; set; } = new List<WeatherRecord>();
        public ICollection<CityAlert> CityAlerts { get; set; } = new List<CityAlert>();
    }
}