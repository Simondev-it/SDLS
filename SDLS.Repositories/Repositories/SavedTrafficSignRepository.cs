using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SavedTrafficSignRepository : GenericRepository<SavedTrafficSign>, ISavedTrafficSignRepository
    {
        public async Task<List<SavedTrafficSign>> GetAllAsync()
        {
            return await _context.SavedTrafficSigns
                .Include(x => x.TrafficSign).ThenInclude(x => x.SignCategory)
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SavedTrafficSign?> GetByIdAsync(Guid id)
        {
            return await _context.SavedTrafficSigns
                .Include(x => x.TrafficSign).ThenInclude(x => x.SignCategory)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<List<SavedTrafficSign>> GetByUserAndTrafficSignAsync(Guid? userId, Guid? trafficSignId)
        {
            var query = _context.SavedTrafficSigns
                .Where(x => x.Status == 1);

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
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
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