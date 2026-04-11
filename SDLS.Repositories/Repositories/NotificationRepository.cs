using Microsoft.EntityFrameworkCore;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public async Task<List<Notification>> GetAllAsync(
            Guid? userId = null,
            string? title = null,
            string? content = null,
            int? status = null,
            string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<Notification> query = isPrivileged
                ? _context.Notifications
                    .Include(n => n.UserNotifications)
                        .ThenInclude(un => un.User)
                : _context.Notifications
                    .Include(n => n.UserNotifications.Where(un => un.Status != 0))
                        .ThenInclude(un => un.User);

            if (userId.HasValue)
                query = query.Where(n => n.UserNotifications.Any(un => un.UserId == userId.Value));

            if (!string.IsNullOrWhiteSpace(title))
            {
                var keyword = title.Trim();
                query = query.Where(n => n.Title != null && EF.Functions.ILike(n.Title, $"%{keyword}%"));
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                var keyword = content.Trim();
                query = query.Where(n => n.Content != null && EF.Functions.ILike(n.Content, $"%{keyword}%"));
            }

            if (status.HasValue)
                query = query.Where(n => n.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!isPrivileged)
            {
                query = query.Where(n =>
                    n.UserNotifications.All(un => un.User == null || un.User.Status != 0));
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(Guid id, string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<Notification> query = isPrivileged
                ? _context.Notifications
                    .Include(n => n.UserNotifications)
                        .ThenInclude(un => un.User)
                : _context.Notifications
                    .Include(n => n.UserNotifications.Where(un => un.Status != 0))
                        .ThenInclude(un => un.User);

            query = query.Where(n => n.Id == id)
                         .ApplyRoleFilter(role);

            if (!isPrivileged)
            {
                query = query.Where(n =>
                    n.UserNotifications.All(un => un.User == null || un.User.Status != 0));
            }

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<Notification?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.Notifications
                .Include(n => n.UserNotifications)
                .FirstOrDefaultAsync(n => n.Id == id);
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

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.Status == 1);
            if (existing == null) return;

            existing.Status = 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.Notifications
                .Include(n => n.UserNotifications)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (existing == null) return;

            if (existing.UserNotifications.Any())
                _context.UserNotifications.RemoveRange(existing.UserNotifications);

            _context.Notifications.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}