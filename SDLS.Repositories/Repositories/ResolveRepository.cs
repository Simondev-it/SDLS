using Microsoft.EntityFrameworkCore;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class ResolveRepository : GenericRepository<Resolve>, IResolveRepository
    {
        public async Task<List<Resolve>> GetAllAsync(
            Guid? id = null,
            Guid? reportId = null,
            Guid? userId = null,
            string? title = null,
            string? content = null,
            int? status = null,
            string? role = null)
        {
            var query = _context.Resolves.AsQueryable();

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (reportId.HasValue)
                query = query.Where(x => x.ReportId == reportId.Value);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(title))
            {
                var keyword = title.Trim();
                query = query.Where(x => x.Title != null && EF.Functions.ILike(x.Title, $"%{keyword}%"));
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                var keyword = content.Trim();
                query = query.Where(x => x.Content != null && EF.Functions.ILike(x.Content, $"%{keyword}%"));
            }

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<Resolve?> GetByIdAsync(Guid id, string? role = null)
        {
            return await _context.Resolves
                .Where(x => x.Id == id)
                .ApplyRoleFilter(role)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<Resolve?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.Resolves
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(Resolve entity)
        {
            await _context.Resolves.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Resolve entity)
        {
            await _context.SaveChangesAsync();
        }


        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.Resolves
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.Resolves
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            _context.Resolves.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}