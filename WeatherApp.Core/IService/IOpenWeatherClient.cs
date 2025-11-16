namespace WeatherApp.Core.IService
{
    public interface IOpenWeatherClient
    {
        /// <summary>
        /// Fetch current weather data from OpenWeatherMap API for a city by coordinates
        /// </summary>
        Task<OpenWeatherResponse?> GetCurrentWeatherAsync(decimal latitude, decimal longitude);
    }

    /// <summary>
    /// Response from OpenWeatherMap API
    /// </summary>
    public class OpenWeatherResponse
    {
        public decimal Temperature { get; set; }
        public decimal FeelsLike { get; set; }
        public int Humidity { get; set; }
        public decimal Pressure { get; set; }
        public decimal WindSpeed { get; set; }
        public decimal? WindDegree { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
