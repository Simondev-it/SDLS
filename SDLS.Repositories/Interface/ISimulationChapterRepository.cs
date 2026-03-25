using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISimulationChapterRepository
    {
        Task<List<SimulationChapter>> GetAllAsync();
        Task<SimulationChapter?> GetByIdAsync(Guid id);
        Task<SimulationChapter?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(SimulationChapter entity);
        Task UpdateAsync(SimulationChapter entity);

        Task DeleteAsync(Guid id);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}