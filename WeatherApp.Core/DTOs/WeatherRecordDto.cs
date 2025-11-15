using System;

namespace WeatherApp.Core.DTOs
{
    public class WeatherRecordDto
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public string? CityName { get; set; }
        public DateTime ObservationTime { get; set; }
        public decimal Temperature { get; set; }
        public decimal? FeelsLike { get; set; }
        public int Humidity { get; set; }
        public decimal? WindSpeed { get; set; }
        public string? WindDirection { get; set; }
        public decimal? Pressure { get; set; }
        public string? Condition { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}