using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SimulationChapter;

namespace SDLS.Services.Interfaces
{
    public interface ISimulationChapterService
    {
        Task<List<SimulationChapterDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null);

        Task<PagedResult<SimulationChapterDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int page = 1,
            int pageSize = 20);

        Task<SimulationChapterDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(SimulationChapterCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, SimulationChapterUpdateDTO dto);

        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}