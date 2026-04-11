using Microsoft.EntityFrameworkCore;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SavedTrafficSignRepository : GenericRepository<SavedTrafficSign>, ISavedTrafficSignRepository
    {
        public async Task<List<SavedTrafficSign>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? trafficSignId = null,
            int? status = null,
            string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<SavedTrafficSign> query = _context.SavedTrafficSigns
                .Include(x => x.TrafficSign)
                    .ThenInclude(x => x.SignCategory);

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (trafficSignId.HasValue)
                query = query.Where(x => x.TrafficSignId == trafficSignId.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!isPrivileged)
            {
                query = query.Where(x =>
                    (x.TrafficSign == null || x.TrafficSign.Status != 0) &&
                    (x.TrafficSign == null || x.TrafficSign.SignCategory == null || x.TrafficSign.SignCategory.Status != 0));
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<SavedTrafficSign?> GetByIdAsync(Guid id, string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            var query = _context.SavedTrafficSigns
                .Include(x => x.TrafficSign)
                    .ThenInclude(x => x.SignCategory)
                .Where(x => x.Id == id)
                .ApplyRoleFilter(role);

            if (!isPrivileged)
            {
                query = query.Where(x =>
                    (x.TrafficSign == null || x.TrafficSign.Status != 0) &&
                    (x.TrafficSign == null || x.TrafficSign.SignCategory == null || x.TrafficSign.SignCategory.Status != 0));
            }

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<List<SavedTrafficSign>> GetByUserAndTrafficSignAsync(Guid? userId, Guid? trafficSignId)
        {
            IQueryable<SavedTrafficSign> query = _context.SavedTrafficSigns;

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (trafficSignId.HasValue)
                query = query.Where(x => x.TrafficSignId == trafficSignId.Value);

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(SavedTrafficSign entity)
        {
            await _context.SavedTrafficSigns.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SavedTrafficSign entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.SavedTrafficSigns
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var entity = await _context.SavedTrafficSigns
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            _context.SavedTrafficSigns.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}