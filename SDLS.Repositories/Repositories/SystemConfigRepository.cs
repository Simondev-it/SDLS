using Microsoft.EntityFrameworkCore;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SystemConfigRepository : GenericRepository<SystemConfig>, ISystemConfigRepository
    {
        public async Task<List<SystemConfig>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            int? value = null,
            string? description = null,
            int? status = null,
            string? role = null)
        {
            var query = _context.SystemConfigs.AsQueryable();

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (!string.IsNullOrWhiteSpace(name))
            {
                var keyword = name.Trim();
                query = query.Where(x => x.Name != null && EF.Functions.ILike(x.Name, $"%{keyword}%"));
            }

            if (value.HasValue)
                query = query.Where(x => x.Value == value.Value);

            if (!string.IsNullOrWhiteSpace(description))
            {
                var keyword = description.Trim();
                query = query.Where(x => x.Description != null && EF.Functions.ILike(x.Description, $"%{keyword}%"));
            }

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<SystemConfig?> GetByIdAsync(Guid id, string? role = null)
        {
            return await _context.SystemConfigs
                .Where(x => x.Id == id)
                .ApplyRoleFilter(role)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<SystemConfig?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.SystemConfigs.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(SystemConfig entity)
        {
            await _context.SystemConfigs.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SystemConfig entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.SystemConfigs.FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
            if (existing == null) return;

            existing.Status = 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.SystemConfigs.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return;

            _context.SystemConfigs.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
