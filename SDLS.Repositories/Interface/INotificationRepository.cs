using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetAllAsync(
            Guid? userId = null,
            string? title = null,
            string? content = null,
            int? status = null,
            string? role = null);

        Task<Notification?> GetByIdAsync(Guid id, string? role = null);
        Task<Notification?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(Notification entity);
        Task UpdateAsync(Notification entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}