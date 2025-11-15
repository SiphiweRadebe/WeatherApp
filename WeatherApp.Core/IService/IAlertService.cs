using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApp.Core.DTOs;

namespace WeatherApp.Core.IService
{
    public interface IAlertService
    {
        Task<AlertDto> GetByIdAsync(int id);
        Task<IEnumerable<AlertDto>> GetAllActiveAsync();
        Task<IEnumerable<AlertDto>> GetByCityIdAsync(int cityId);
        Task<AlertDto> CreateAsync(CreateAlertDto dto);
        Task<AlertDto> UpdateAsync(int id, UpdateAlertDto dto);
        Task<bool> DeleteAsync(int id);
    }
}