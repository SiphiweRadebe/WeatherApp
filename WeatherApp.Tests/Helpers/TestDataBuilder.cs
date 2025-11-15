using WeatherApp.Core.DTOs;
using WeatherApp.Data.Entities;

namespace WeatherApp.Tests.Helpers
{
    public static class TestDataBuilder
    {
        public static City CreateCity(
            int id = 1,
            string name = "TestCity",
            string country = "TestCountry")
        {
            return new City
            {
                Id = id,
                Name = name,
                Country = country,
                Latitude = 51.5074m,
                Longitude = -0.1278m,
                TimeZone = "Europe/London",
                CreatedAt = DateTime.UtcNow
            };
        }

        public static CreateCityDto CreateCityDto(
            string name = "TestCity",
            string country = "TestCountry")
        {
            return new CreateCityDto
            {
                Name = name,
                Country = country,
                Latitude = 51.5074m,
                Longitude = -0.1278m,
                TimeZone = "Europe/London"
            };
        }

        public static WeatherRecord CreateWeatherRecord(
            int id = 1,
            int cityId = 1,
            decimal temperature = 20.5m)
        {
            return new WeatherRecord
            {
                Id = id,
                CityId = cityId,
                ObservationTime = DateTime.UtcNow,
                Temperature = temperature,
                FeelsLike = temperature - 1,
                Humidity = 65,
                WindSpeed = 10.5m,
                WindDirection = "NW",
                Pressure = 1013.25m,
                Condition = "Cloudy",
                Description = "Test weather",
                CreatedAt = DateTime.UtcNow
            };
        }

        public static CreateWeatherRecordDto CreateWeatherRecordDto(
            int cityId = 1,
            decimal temperature = 20.5m)
        {
            return new CreateWeatherRecordDto
            {
                CityId = cityId,
                ObservationTime = DateTime.UtcNow,
                Temperature = temperature,
                FeelsLike = temperature - 1,
                Humidity = 65,
                WindSpeed = 10.5m,
                WindDirection = "NW",
                Pressure = 1013.25m,
                Condition = "Cloudy",
                Description = "Test weather"
            };
        }
    }
}