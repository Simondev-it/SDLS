using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetAllAsync();
        Task<Notification?> GetByIdAsync(Guid id);
        Task<Notification?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(Notification entity);
        Task UpdateAsync(Notification entity);

        Task DeleteAsync(Guid id);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}