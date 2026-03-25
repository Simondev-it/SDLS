using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISimulationCategoryRepository
    {
        Task<List<SimulationCategory>> GetAllAsync();
        Task<SimulationCategory?> GetByIdAsync(Guid id);
        Task<SimulationCategory?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(SimulationCategory entity);
        Task UpdateAsync(SimulationCategory entity);

        Task DeleteAsync(Guid id);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}