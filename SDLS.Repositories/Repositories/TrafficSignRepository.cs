using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
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
            int? status = 1)
        {
            var query = _context.TrafficSigns
                .Include(x => x.SignCategory)
                .AsNoTracking()
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

            return await query.ToListAsync();
        }

        public async Task<TrafficSign?> GetByIdAsync(Guid id)
        {
            return await _context.TrafficSigns
                .Include(x => x.SignCategory)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<TrafficSign?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.TrafficSigns
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
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

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _context.TrafficSigns
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }
    }
}