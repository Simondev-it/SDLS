using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISimulationCategoryRepository
    {
        Task<List<SimulationCategory>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? role = null);

        Task<SimulationCategory?> GetByIdAsync(Guid id, string? role = null);
        Task<SimulationCategory?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(SimulationCategory entity);
        Task UpdateAsync(SimulationCategory entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}