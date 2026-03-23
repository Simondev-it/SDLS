using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SimulationScenario;

namespace SDLS.Services.Interfaces
{
    public interface ISimulationScenarioService
    {
        Task<PagedResult<SimulationScenarioDTO>> GetAllAsync(
            Guid? simulationCategoryId = null,
            Guid? simulationChapterId = null,
            Guid? simulationDifficultyLevelId = null,
            string? name = null,
            int page = 1,
            int pageSize = 20);

        Task<SimulationScenarioDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(SimulationScenarioCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, SimulationScenarioUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}