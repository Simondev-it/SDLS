using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SimulationDifficultyLevel;

namespace SDLS.Services.Interfaces
{
    public interface ISimulationDifficultyLevelService
    {
        Task<List<SimulationDifficultyLevelDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null);

        Task<PagedResult<SimulationDifficultyLevelDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int page = 1,
            int pageSize = 20);

        Task<SimulationDifficultyLevelDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(SimulationDifficultyLevelCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, SimulationDifficultyLevelUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}