using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISimulationChapterRepository
    {
        Task<List<SimulationChapter>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? role = null);

        Task<SimulationChapter?> GetByIdAsync(Guid id, string? role = null);
        Task<SimulationChapter?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(SimulationChapter entity);
        Task UpdateAsync(SimulationChapter entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}