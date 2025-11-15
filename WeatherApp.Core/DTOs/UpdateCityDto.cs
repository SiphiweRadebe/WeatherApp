namespace WeatherApp.Core.DTOs
{
    public class UpdateCityDto
    {
        public string? Name { get; set; }
        public string? Country { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? TimeZone { get; set; }
    }
}