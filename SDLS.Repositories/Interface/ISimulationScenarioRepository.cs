using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISimulationScenarioRepository
    {
        Task<IEnumerable<SimulationScenario>> GetAllAsync();
        Task<SimulationScenario?> GetByIdAsync(Guid id);
        Task<SimulationScenario?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(SimulationScenario entity);
        Task UpdateAsync(SimulationScenario entity);
        Task DeleteAsync(Guid id);
    }
}