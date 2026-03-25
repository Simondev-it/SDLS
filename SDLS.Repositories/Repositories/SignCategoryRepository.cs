using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SignCategoryRepository : GenericRepository<SignCategory>, ISignCategoryRepository
    {
        public async Task<List<SignCategory>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? role = null)
        {
            var query = _context.SignCategories.AsQueryable();

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (!string.IsNullOrWhiteSpace(name))
            {
                var keyword = name.Trim();
                query = query.Where(x => x.Name != null && EF.Functions.ILike(x.Name, $"%{keyword}%"));
            }

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

        public async Task<SignCategory?> GetByIdAsync(Guid id, string? role = null)
        {
            return await _context.SignCategories
                .Where(x => x.Id == id)
                .ApplyRoleFilter(role)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<SignCategory?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.SignCategories
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(SignCategory entity)
        {
            await _context.SignCategories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SignCategory entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.SignCategories
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.SignCategories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            _context.SignCategories.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}