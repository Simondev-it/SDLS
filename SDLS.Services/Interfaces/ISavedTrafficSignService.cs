using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SavedTrafficSign;

namespace SDLS.Services.Interfaces
{
    public interface ISavedTrafficSignService
    {
        Task<List<SavedTrafficSignDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? trafficSignId = null,
            int? status = null);

        Task<PagedResult<SavedTrafficSignDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? trafficSignId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<SavedTrafficSignDTO?> GetByIdAsync(Guid id);
        Task<SavedTrafficSignDTO> CreateAsync(SavedTrafficSignCreateDTO dto);
        Task<SavedTrafficSignDTO> UpdateAsync(Guid id, SavedTrafficSignUpdateDTO dto);
        Task<SavedTrafficSignDTO> DeleteSoftAsync(Guid id);
        Task<SavedTrafficSignDTO> DeleteHardAsync(Guid id);
    }
}