using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public async Task<List<Notification>> GetAllAsync()
        {
            return await _context.Notifications
                .Include(n => n.UserNotifications.Where(un => un.Status == 1))
                .Where(n => n.Status == 1 || n.Status == 2)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications
                .Include(n => n.UserNotifications.Where(un => un.Status == 1))
                    .ThenInclude(un => un.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == id && (n.Status == 1 || n.Status == 2));
        }

        public async Task<Notification?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.Notifications
                .Include(n => n.UserNotifications)
                .FirstOrDefaultAsync(n => n.Id == id && (n.Status == 1 || n.Status == 2));
        }

        public async Task AddAsync(Notification entity)
        {
            await _context.Notifications.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Notification entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.Status == 1);

            if (existing == null) return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }
    }
}