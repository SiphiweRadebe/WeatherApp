using System;
using System.Collections.Generic;

namespace WeatherApp.Core.DTOs
{
    public class CreateAlertDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string AlertType { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<int>? CityIds { get; set; }
    }
}
