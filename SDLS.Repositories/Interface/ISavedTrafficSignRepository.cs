using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISavedTrafficSignRepository
    {
        Task<List<SavedTrafficSign>> GetAllAsync();
        Task<SavedTrafficSign?> GetByIdAsync(Guid id);
        Task<List<SavedTrafficSign>> GetByUserAndTrafficSignAsync(Guid? userId, Guid? trafficSignId);
        Task AddAsync(SavedTrafficSign entity);
        Task UpdateAsync(SavedTrafficSign entity);

        Task DeleteAsync(Guid id);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}