using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISavedTrafficSignRepository
    {
        Task<List<SavedTrafficSign>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? trafficSignId = null,
            int? status = null,
            string? role = null);

        Task<SavedTrafficSign?> GetByIdAsync(Guid id, string? role = null);
        Task<List<SavedTrafficSign>> GetByUserAndTrafficSignAsync(Guid? userId, Guid? trafficSignId);
        Task AddAsync(SavedTrafficSign entity);
        Task UpdateAsync(SavedTrafficSign entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}