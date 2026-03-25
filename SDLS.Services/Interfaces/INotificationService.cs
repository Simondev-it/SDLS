using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Notification;

namespace SDLS.Services.Interfaces
{
    public interface INotificationService
    {
        Task<PagedResult<NotificationDTO>> GetAllAsync(
            Guid? userId = null,
            string? title = null,
            string? content = null,
            string? sortBy = "time",
            int page = 1,
            int pageSize = 20);

        Task<NotificationDTO?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(NotificationCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, NotificationUpdateDTO dto);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}