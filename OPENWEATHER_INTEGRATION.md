# OpenWeatherMap Integration Guide

## Overview
This WeatherApp now includes integration with the OpenWeatherMap API to fetch real-time weather data for cities.

## Setup

### 1. API Key Configuration
The OpenWeatherMap API key is stored in the configuration files:

- **Development**: `WeatherApp.ApiService/appsettings.Development.json`
- **Production**: Use environment variables or `appsettings.json`

The key is already configured in development:
```json
{
  "OpenWeatherMap": {
    "ApiKey": "092fea5192ba972136da7ff725794b51"
  }
}
```

**Important**: Never commit sensitive API keys to source control. For production:
1. Use Azure Key Vault or similar secure storage
2. Set via environment variables: `OpenWeatherMap:ApiKey`
3. Use `dotnet user-secrets` for local development

## Features

### 1. Fetch and Save Weather Data
**Endpoint**: `POST /api/weather/fetch-and-save/{cityId}`

Fetches current weather from OpenWeatherMap API and automatically saves it to the database.

**Example**: 
- Button: "Fetch from OpenWeather" on the Weather page
- Returns: `WeatherRecordDto` with saved data

**Response**:
```json
{
  "id": 123,
  "cityId": 1,
  "temperature": 22.3,
  "feelsLike": 23.1,
  "humidity": 65,
  "windSpeed": 8.2,
  "windDirection": "NE",
  "pressure": 1015.50,
  "condition": "Sunny",
  "description": "Clear skies with gentle breeze",
  "observationTime": "2024-01-15T14:30:00Z"
}
```

### 2. Get Current Weather Only
**Endpoint**: `GET /api/weather/current/{cityId}`

Fetches current weather without saving to database.

**Response**:
```json
{
  "temperature": 22.3,
  "feelsLike": 23.1,
  "humidity": 65,
  "windSpeed": 8.2,
  "windDegree": 45,
  "condition": "Sunny",
  "description": "Clear skies with gentle breeze",
  "pressure": 1015.50
}
```

## UI Integration

### Weather Page (`/weather/{cityId}`)
- **"Fetch from OpenWeather" Button**: Fetches real-time data and saves to database
- **Latest Weather Section**: Displays most recent weather data
- **Weather History**: Shows all historical records

## Architecture

### Components
1. **IOpenWeatherClient** (Interface)
   - Location: `WeatherApp.Core/IService/IOpenWeatherClient.cs`
   - Defines contract for OpenWeatherMap API calls

2. **OpenWeatherClient** (Implementation)
   - Location: `WeatherApp.Core/Services/OpenWeatherClient.cs`
   - Handles API communication
   - Maps OpenWeatherMap response to internal DTOs
   - Converts wind degrees to cardinal directions

3. **WeatherController**
   - Location: `WeatherApp.ApiService/Controllers/WeatherController.cs`
   - Exposes REST endpoints
   - Orchestrates data fetch and save operations

4. **WeatherApiClient**
   - Location: `WeatherApp.Web/Services/WeatherApiClient.cs`
   - Blazor UI client for calling backend endpoints

## Data Mapping

### OpenWeatherMap API Response ? Internal DTO
The client automatically maps OpenWeatherMap's response to your internal `WeatherRecordDto`:
- `main.temp` ? `Temperature` (in Celsius)
- `main.feels_like` ? `FeelsLike` (in Celsius)
- `main.humidity` ? `Humidity` (as percentage)
- `main.pressure` ? `Pressure` (hPa)
- `wind.speed` ? `WindSpeed` (m/s converted to km/h where needed)
- `wind.deg` ? `WindDegree` (converted to cardinal direction: N, NNE, NE, etc.)
- `weather[0].main` ? `Condition` (e.g., "Sunny", "Cloudy", "Rainy")
- `weather[0].description` ? `Description` (detailed description)

## Error Handling
- 404 if city not found
- 500 if OpenWeatherMap API fails
- All errors are logged with context
- Graceful fallbacks with appropriate HTTP status codes

## Logging
All OpenWeatherMap operations are logged with:
- Request details (latitude, longitude)
- API response status
- Errors and warnings
- Level: Information/Warning/Error

## Rate Limiting
OpenWeatherMap Free tier: 60 requests/minute
- Implement caching if needed (use `IMemoryCache`)
- Consider adding exponential backoff with Polly

## Testing
Example curl commands:

```bash
# Fetch and save weather for city ID 1
curl -X POST https://localhost:7001/api/weather/fetch-and-save/1

# Get current weather only for city ID 1
curl -X GET https://localhost:7001/api/weather/current/1
```

## Troubleshooting

### "OpenWeatherMap API key is not configured"
- Check `appsettings.json` or environment variables
- Ensure key is under `OpenWeatherMap:ApiKey` path

### "City not found"
- Verify city exists in database
- Check the `cityId` parameter

### "Failed to fetch weather data"
- Verify API key is valid
- Check internet connectivity
- Verify coordinates are valid (latitude: -90 to 90, longitude: -180 to 180)

## Future Enhancements
1. Add caching with `IMemoryCache` for improved performance
2. Implement retry logic with Polly
3. Add forecast data (5-day, hourly)
4. Add weather alerts from OpenWeatherMap
5. Batch fetch for multiple cities
6. Rate limit handling
