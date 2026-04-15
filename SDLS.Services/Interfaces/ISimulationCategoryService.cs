using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SimulationCategory;

namespace SDLS.Services.Interfaces
{
    public interface ISimulationCategoryService
    {
        Task<List<SimulationCategoryDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null);

        Task<PagedResult<SimulationCategoryDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<(byte[] Content, string FileName, string ContentType)> ExportToExcelAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null);

        Task<SimulationCategoryDTO> GetByIdAsync(Guid id);
        Task<SimulationCategoryDTO> CreateAsync(SimulationCategoryCreateDTO dto);
        Task<SimulationCategoryDTO> UpdateAsync(Guid id, SimulationCategoryUpdateDTO dto);
        Task<SimulationCategoryDTO> DeleteSoftAsync(Guid id);
        Task<SimulationCategoryDTO> DeleteHardAsync(Guid id);
    }
}