using Bogus;
using WeatherApp.Web.Models;
using WeatherApp.Web.Services;

namespace WeatherApp.Tests.Fakers
{
    public static class WeatherFakers
    {
        public static Faker<CityViewModel> CityViewModelFaker => new Faker<CityViewModel>()
            .RuleFor(c => c.Id, f => f.Random.Int(1, 1000))
            .RuleFor(c => c.Name, f => f.Address.City())
            .RuleFor(c => c.Country, f => f.Address.CountryCode())
            .RuleFor(c => c.Latitude, f => (decimal)f.Address.Latitude())
            .RuleFor(c => c.Longitude, f => (decimal)f.Address.Longitude())
            .RuleFor(c => c.TimeZone, f => f.Date.TimeZoneString())
            .RuleFor(c => c.CreatedAt, f => f.Date.Past())
            .RuleFor(c => c.WeatherRecords, f => null);

        public static Faker<CreateCityRequest> CreateCityRequestFaker => new Faker<CreateCityRequest>()
            .RuleFor(c => c.Name, f => f.Address.City())
            .RuleFor(c => c.Country, f => f.Address.CountryCode())
            .RuleFor(c => c.Latitude, f => (decimal)f.Address.Latitude())
            .RuleFor(c => c.Longitude, f => (decimal)f.Address.Longitude())
            .RuleFor(c => c.TimeZone, f => f.Date.TimeZoneString());

        public static Faker<WeatherRecordViewModel> WeatherRecordViewModelFaker => new Faker<WeatherRecordViewModel>()
            .RuleFor(w => w.Id, f => f.Random.Int(1, 10000))
            .RuleFor(w => w.CityId, f => f.Random.Int(1, 1000))
            .RuleFor(w => w.CityName, f => f.Address.City())
            .RuleFor(w => w.ObservationTime, f => f.Date.Recent())
            .RuleFor(w => w.Temperature, f => f.Random.Decimal(-30, 50))
            .RuleFor(w => w.FeelsLike, f => f.Random.Decimal(-30, 50))
            .RuleFor(w => w.Humidity, f => f.Random.Int(0, 100))
            .RuleFor(w => w.WindSpeed, f => f.Random.Decimal(0, 100))
            .RuleFor(w => w.WindDirection, f => f.PickRandom("N", "NE", "E", "SE", "S", "SW", "W", "NW"))
            .RuleFor(w => w.Pressure, f => f.Random.Decimal(980, 1050))
            .RuleFor(w => w.Condition, f => f.PickRandom("Clear", "Cloudy", "Rainy", "Snowy", "Stormy"))
            .RuleFor(w => w.Description, f => f.Lorem.Sentence());

        public static Faker<CreateWeatherRecordRequest> CreateWeatherRecordRequestFaker => new Faker<CreateWeatherRecordRequest>()
            .RuleFor(w => w.CityId, f => f.Random.Int(1, 1000))
            .RuleFor(w => w.Temperature, f => f.Random.Decimal(-30, 50))
            .RuleFor(w => w.FeelsLike, f => f.Random.Decimal(-30, 50))
            .RuleFor(w => w.Humidity, f => f.Random.Int(0, 100))
            .RuleFor(w => w.WindSpeed, f => f.Random.Decimal(0, 100))
            .RuleFor(w => w.WindDirection, f => f.PickRandom("N", "NE", "E", "SE", "S", "SW", "W", "NW"))
            .RuleFor(w => w.Pressure, f => f.Random.Decimal(980, 1050))
            .RuleFor(w => w.Condition, f => f.PickRandom("Clear", "Cloudy", "Rainy", "Snowy", "Stormy"))
            .RuleFor(w => w.Description, f => f.Lorem.Sentence());

        public static Faker<AlertViewModel> AlertViewModelFaker => new Faker<AlertViewModel>()
            .RuleFor(a => a.Id, f => f.Random.Int(1, 1000))
            .RuleFor(a => a.Title, f => f.Lorem.Sentence(3))
            .RuleFor(a => a.Description, f => f.Lorem.Paragraph())
            .RuleFor(a => a.Severity, f => f.PickRandom("Low", "Medium", "High", "Critical"))
            .RuleFor(a => a.AlertType, f => f.PickRandom("Temperature", "Storm", "Flood", "Wind", "Snow"))
            .RuleFor(a => a.StartTime, f => f.Date.Recent())
            .RuleFor(a => a.EndTime, f => f.Date.Soon())
            .RuleFor(a => a.IsActive, f => f.Random.Bool())
            .RuleFor(a => a.AffectedCities, f => f.Make(f.Random.Int(1, 5), () => f.Address.City()).ToList());

        public static Faker<OpenWeatherData> OpenWeatherDataFaker => new Faker<OpenWeatherData>()
            .RuleFor(o => o.Temperature, f => f.Random.Decimal(-30, 50))
            .RuleFor(o => o.FeelsLike, f => f.Random.Decimal(-30, 50))
            .RuleFor(o => o.Humidity, f => f.Random.Int(0, 100))
            .RuleFor(o => o.Pressure, f => f.Random.Decimal(980, 1050))
            .RuleFor(o => o.WindSpeed, f => f.Random.Decimal(0, 100))
            .RuleFor(o => o.WindDegree, f => f.Random.Decimal(0, 360))
            .RuleFor(o => o.Condition, f => f.PickRandom("Clear", "Cloudy", "Rainy", "Snowy", "Stormy"))
            .RuleFor(o => o.Description, f => f.Lorem.Sentence());
    }
}
