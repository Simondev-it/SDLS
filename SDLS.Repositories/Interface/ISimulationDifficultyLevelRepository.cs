using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISimulationDifficultyLevelRepository
    {
        Task<List<SimulationDifficultyLevel>> GetAllAsync();
        Task<SimulationDifficultyLevel?> GetByIdAsync(Guid id);
        Task<SimulationDifficultyLevel?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(SimulationDifficultyLevel entity);
        Task UpdateAsync(SimulationDifficultyLevel entity);
        Task DeleteAsync(Guid id);
    }
}