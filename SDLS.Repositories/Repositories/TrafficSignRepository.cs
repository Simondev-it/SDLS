using Microsoft.EntityFrameworkCore;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class TrafficSignRepository : GenericRepository<TrafficSign>, ITrafficSignRepository
    {
        public async Task<IEnumerable<TrafficSign>> GetAllAsync(
            Guid? id = null,
            Guid? signCategoryId = null,
            string? name = null,
            string? code = null,
            string? description = null,
            int? status = null,
            string? role = null)
        {
            var query = _context.TrafficSigns
                .Include(x => x.SignCategory)
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (signCategoryId.HasValue)
                query = query.Where(x => x.SignCategoryId == signCategoryId.Value);

            if (!string.IsNullOrWhiteSpace(name))
            {
                var keyword = name.Trim();
                query = query.Where(x => x.Name != null && EF.Functions.ILike(x.Name, $"%{keyword}%"));
            }

            if (!string.IsNullOrWhiteSpace(code))
            {
                var keyword = code.Trim();
                query = query.Where(x => x.Code != null && EF.Functions.ILike(x.Code, $"%{keyword}%"));
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var keyword = description.Trim();
                query = query.Where(x => x.Description != null && EF.Functions.ILike(x.Description, $"%{keyword}%"));
            }

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
                query = query.Where(x => x.SignCategory == null || x.SignCategory.Status != 0);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<TrafficSign?> GetByIdAsync(Guid id, string? role = null)
        {
            var query = _context.TrafficSigns
                .Include(x => x.SignCategory)
                .Where(x => x.Id == id)
                .ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
                query = query.Where(x => x.SignCategory == null || x.SignCategory.Status != 0);

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<TrafficSign?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.TrafficSigns
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(TrafficSign entity)
        {
            await _context.TrafficSigns.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TrafficSign entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.TrafficSigns
                .FirstOrDefaultAsync(x => x.Id == id && (x.Status == 1 || x.Status == 0));

            if (existing == null)
                return;

            existing.Status = existing.Status == 0 ? 1 : 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.TrafficSigns
                .Include(x => x.SavedTrafficSigns)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            if (existing.SavedTrafficSigns.Any())
                _context.SavedTrafficSigns.RemoveRange(existing.SavedTrafficSigns);

            _context.TrafficSigns.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}