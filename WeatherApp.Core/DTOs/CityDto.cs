using System;
using System.Collections.Generic;

namespace WeatherApp.Core.DTOs
{
    public class CityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? TimeZone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<WeatherRecordDto>? WeatherRecords { get; set; }
        public List<AlertDto>? Alerts { get; set; }
    }
}