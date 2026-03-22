using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class ReportCategoryRepository : GenericRepository<ReportCategory>, IReportCategoryRepository
    {
        public async Task<List<ReportCategory>> GetAllAsync()
        {
            return await _context.ReportCategories
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ReportCategory?> GetByIdAsync(Guid id)
        {
            return await _context.ReportCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<ReportCategory?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.ReportCategories
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task AddAsync(ReportCategory entity)
        {
            await _context.ReportCategories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ReportCategory entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _context.ReportCategories.FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
            if (existing == null) return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();

            await _context.SaveChangesAsync();
        }
    }
}