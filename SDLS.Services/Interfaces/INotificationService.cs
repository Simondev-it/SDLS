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
            int? status = null,
            string? sortBy = "time",
            int page = 1,
            int pageSize = 20);

        Task<NotificationDTO?> GetByIdAsync(Guid id);
        Task<NotificationDTO> CreateAsync(NotificationCreateDTO dto);
        Task<NotificationDTO> UpdateAsync(Guid id, NotificationUpdateDTO dto);
        Task<NotificationDTO> DeleteSoftAsync(Guid id);
        Task<NotificationDTO> DeleteHardAsync(Guid id);
    }
}