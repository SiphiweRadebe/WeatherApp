using System;

namespace WeatherApp.Core.DTOs
{
    public class UpdateAlertDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Severity { get; set; }
        public DateTime? EndTime { get; set; }
        public bool? IsActive { get; set; }
    }
}