using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISimulationDifficultyLevelRepository
    {
        Task<List<SimulationDifficultyLevel>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? role = null);

        Task<SimulationDifficultyLevel?> GetByIdAsync(Guid id, string? role = null);
        Task<SimulationDifficultyLevel?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(SimulationDifficultyLevel entity);
        Task UpdateAsync(SimulationDifficultyLevel entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}