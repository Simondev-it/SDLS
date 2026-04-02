using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISimulationScenarioRepository
    {
        Task<IEnumerable<SimulationScenario>> GetAllAsync(
            Guid? simulationCategoryId = null,
            Guid? simulationChapterId = null,
            Guid? simulationDifficultyLevelId = null,
            string? name = null,
            int? status = null,
            string? role = null);

        Task<SimulationScenario?> GetByIdAsync(Guid id, string? role = null);
        Task<SimulationScenario?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(SimulationScenario entity);
        Task UpdateAsync(SimulationScenario entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}