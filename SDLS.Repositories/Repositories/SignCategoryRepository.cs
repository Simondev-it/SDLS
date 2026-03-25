using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SignCategoryRepository : GenericRepository<SignCategory>, ISignCategoryRepository
    {
        public async Task<List<SignCategory>> GetAllAsync()
        {
            return await _context.SignCategories
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SignCategory?> GetByIdAsync(Guid id)
        {
            return await _context.SignCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<SignCategory?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.SignCategories
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
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