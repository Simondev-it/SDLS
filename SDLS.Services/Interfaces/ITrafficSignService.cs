using SDLS.Model.DTOs;
using SDLS.Model.DTOs.TrafficSign;

namespace SDLS.Services.Interfaces
{
    public interface ITrafficSignService
    {
        Task<PagedResult<TrafficSignDTO>> GetAllAsync(
            Guid? id = null,
            Guid? signCategoryId = null,
            string? name = null,
            string? code = null,
            string? description = null,
            int? status = 1,
            int page = 1,
            int pageSize = 20);

        Task<TrafficSignDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(TrafficSignCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, TrafficSignUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}