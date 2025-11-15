using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApp.Core.DTOs;

namespace WeatherApp.Core.IService
{
    public interface ICityService
    {
        Task<CityDto> GetByIdAsync(int id);
        Task<IEnumerable<CityDto>> GetAllAsync();
        Task<CityDto> CreateAsync(CreateCityDto dto);
        Task<CityDto> UpdateAsync(int id, UpdateCityDto dto);
        Task<bool> DeleteAsync(int id);
        Task<CityDto> GetCityWithWeatherAsync(int id);
    }
}