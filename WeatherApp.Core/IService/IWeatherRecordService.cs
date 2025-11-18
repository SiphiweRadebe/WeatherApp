using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApp.Core.DTOs;

namespace WeatherApp.Core.IService
{
    public interface IWeatherRecordService
    {
        Task<WeatherRecordDto> GetByIdAsync(int id);
        Task<IEnumerable<WeatherRecordDto>> GetByCityIdAsync(int cityId);
        Task<WeatherRecordDto?> GetLatestByCityIdAsync(int cityId);
        Task<WeatherRecordDto> CreateAsync(CreateWeatherRecordDto dto);
        Task<bool> DeleteAsync(int id);
        Task<WeatherRecordDto?> FetchFromOpenWeatherAsync(int cityId);
    }
}