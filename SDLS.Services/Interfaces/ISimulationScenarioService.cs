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
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<SimulationScenarioDTO> GetByIdAsync(Guid id);
        Task<SimulationScenarioDTO> CreateAsync(SimulationScenarioCreateDTO dto);
        Task<List<SimulationScenarioDTO>> CreateManyAsync(List<SimulationScenarioCreateDTO> dtos);
        Task<SimulationScenarioDTO> UpdateAsync(Guid id, SimulationScenarioUpdateDTO dto);
        Task<SimulationScenarioDTO> DeleteSoftAsync(Guid id);
        Task<SimulationScenarioDTO> DeleteHardAsync(Guid id);
    }
}