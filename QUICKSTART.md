# OpenWeatherMap Integration - Quick Start Guide

## Quick Setup (5 minutes)

### 1. Configuration (Already Done! ?)
API key is already configured in development:
```json
// WeatherApp.ApiService/appsettings.Development.json
{
  "OpenWeatherMap": {
    "ApiKey": "092fea5192ba972136da7ff725794b51"
  }
}
```

### 2. Running the Application
```bash
cd WeatherApp
dotnet run
```

### 3. Using the Feature

#### Via UI (Recommended)
1. Navigate to `https://localhost:7001` ? Cities
2. Create a city with name, country, and **coordinates**
3. Click "View Weather"
4. Click blue **"Fetch from OpenWeather"** button
5. ? Weather data automatically fetches and saves

#### Via API (cURL)
```bash
# Fetch weather for city ID 1 and save to database
curl -X POST https://localhost:7001/api/weather/fetch-and-save/1

# Get current weather without saving
curl -X GET https://localhost:7001/api/weather/current/1
```

#### Via Postman
```
POST /api/weather/fetch-and-save/1
GET /api/weather/current/1
```

## Example: Create City with Coordinates

### Create a Test City
Using the "Add City" button:
- **Name**: London
- **Country**: United Kingdom
- **Latitude**: 51.5074
- **Longitude**: -0.1278

Then click "Fetch from OpenWeather" to get real data!

## Response Examples

### Fetch and Save Response (POST)
```json
{
  "id": 10,
  "cityId": 1,
  "cityName": "London",
  "temperature": 12.5,
  "feelsLike": 11.8,
  "humidity": 72,
  "windSpeed": 5.2,
  "windDirection": "SW",
  "pressure": 1013.25,
  "condition": "Cloudy",
  "description": "Overcast clouds",
  "observationTime": "2024-01-15T14:35:00Z",
  "createdAt": "2024-01-15T14:35:42Z"
}
```

### Current Weather Response (GET)
```json
{
  "temperature": 12.5,
  "feelsLike": 11.8,
  "humidity": 72,
  "pressure": 1013.25,
  "windSpeed": 5.2,
  "windDegree": 225,
  "condition": "Cloudy",
  "description": "Overcast clouds"
}
```

## UI Changes

### Weather Page Before
```
[View Weather] for each city
```

### Weather Page After
```
[Fetch from OpenWeather] [Add Weather Data]
Latest Weather Display
Weather History Table
```

The blue button fetches real-time data from OpenWeatherMap!

## Common Cities to Test

Test with these coordinates:

| City | Country | Latitude | Longitude |
|------|---------|----------|-----------|
| New York | USA | 40.7128 | -74.0060 |
| London | UK | 51.5074 | -0.1278 |
| Tokyo | Japan | 35.6762 | 139.6503 |
| Sydney | Australia | -33.8688 | 151.2093 |
| Paris | France | 48.8566 | 2.3522 |
| Dubai | UAE | 25.2048 | 55.2708 |

## Verification Checklist

- ? Solution builds without errors
- ? API key configured in `appsettings.Development.json`
- ? OpenWeatherClient registered in Program.cs
- ? WeatherController endpoints available
- ? "Fetch from OpenWeather" button visible on Weather page
- ? Data persists to database
- ? UI updates after fetch
- ? Error handling functional
- ? Logging operational

## Troubleshooting

### Button doesn't appear
- Rebuild solution: `dotnet clean && dotnet build`
- Clear browser cache
- Restart development server

### "Failed to fetch"
- Check internet connectivity
- Verify city coordinates are valid
- Check API key in `appsettings.Development.json`
- Review logs in Visual Studio Output window

### "City not found"
- Ensure city exists in database
- Use correct cityId in URL
- Create city via UI first

### Different data on each fetch
- This is normal! Real-time data changes
- OpenWeatherMap reflects current conditions

## API Usage (Production)

### Free Tier Limits
- 60 calls/minute
- 1M calls/month
- 5-day forecast available

### For Higher Limits
OpenWeatherMap offers paid plans:
- Professional: 200,000 calls/month
- Enterprise: Unlimited

## Database Changes

After fetching, the `WeatherRecords` table will have new entries:
```sql
SELECT * FROM WeatherRecords 
WHERE CityId = 1 
ORDER BY ObservationTime DESC
```

Each fetch creates a new historical record for trend analysis!

## Performance Tips

1. **Cache Results**: Use `IMemoryCache` for 10-30 minute caches
2. **Batch Updates**: Fetch weather for multiple cities in one call
3. **Scheduled Updates**: Use `BackgroundService` for periodic updates
4. **Rate Limit**: Track API calls to avoid hitting free tier limits

## Next Steps

After verifying it works:
1. Review `OPENWEATHER_INTEGRATION.md` for full documentation
2. Read `IMPLEMENTATION_SUMMARY.md` for architecture details
3. Explore adding forecasts or alerts
4. Consider caching implementation
5. Set up production API key management

## Support Resources

- OpenWeatherMap Docs: https://openweathermap.org/api
- Current Weather API: https://openweathermap.org/current
- API Reference: https://openweathermap.org/weather-conditions

## Key Takeaways

| Feature | Status |
|---------|--------|
| Real-time weather fetch | ? Working |
| Auto-save to database | ? Working |
| Cardinal direction conversion | ? Working |
| Error handling | ? Working |
| UI integration | ? Working |
| Configuration ready | ? Ready |
| Documentation complete | ? Complete |

---

**Ready to use!** Start the app and click "Fetch from OpenWeather" on any city weather page.
