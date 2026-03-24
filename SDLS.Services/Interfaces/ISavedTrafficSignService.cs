using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SavedTrafficSign;

namespace SDLS.Services.Interfaces
{
    public interface ISavedTrafficSignService
    {
        Task<List<SavedTrafficSignDTO>> GetAllAsync(Guid? id = null, Guid? userId = null, Guid? trafficSignId = null);
        Task<PagedResult<SavedTrafficSignDTO>> GetPagedAsync(Guid? id = null, Guid? userId = null, Guid? trafficSignId = null, int page = 1, int pageSize = 20);
        Task<SavedTrafficSignDTO?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(SavedTrafficSignCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, SavedTrafficSignUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}