using System;
using System.Collections.Generic;

namespace WeatherApp.Web.Models
{
    public class CityViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? TimeZone { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<WeatherRecordViewModel>? WeatherRecords { get; set; }
    }

    public class WeatherRecordViewModel
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
    }

    public class AlertViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string AlertType { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; }
        public List<string>? AffectedCities { get; set; }
    }

    public class CreateCityRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? TimeZone { get; set; }
    }

    public class CreateWeatherRecordRequest
    {
        public int CityId { get; set; }
        public decimal Temperature { get; set; }
        public decimal? FeelsLike { get; set; }
        public int Humidity { get; set; }
        public decimal? WindSpeed { get; set; }
        public string? WindDirection { get; set; }
        public decimal? Pressure { get; set; }
        public string? Condition { get; set; }
        public string? Description { get; set; }
    }
}