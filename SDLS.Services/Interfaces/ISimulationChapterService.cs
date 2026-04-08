using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SimulationChapter;

namespace SDLS.Services.Interfaces
{
    public interface ISimulationChapterService
    {
        Task<List<SimulationChapterDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null);

        Task<PagedResult<SimulationChapterDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<SimulationChapterDTO> GetByIdAsync(Guid id);
        Task<SimulationChapterDTO> CreateAsync(SimulationChapterCreateDTO dto);
        Task<SimulationChapterDTO> UpdateAsync(Guid id, SimulationChapterUpdateDTO dto);
        Task<SimulationChapterDTO> DeleteSoftAsync(Guid id);
        Task<SimulationChapterDTO> DeleteHardAsync(Guid id);
    }
}