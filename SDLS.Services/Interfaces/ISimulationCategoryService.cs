using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SimulationCategory;

namespace SDLS.Services.Interfaces
{
    public interface ISimulationCategoryService
    {
        Task<List<SimulationCategoryDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null);

        Task<PagedResult<SimulationCategoryDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int page = 1,
            int pageSize = 20);

        Task<SimulationCategoryDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(SimulationCategoryCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, SimulationCategoryUpdateDTO dto);

        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}