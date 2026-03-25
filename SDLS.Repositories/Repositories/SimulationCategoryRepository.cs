using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SimulationCategoryRepository : GenericRepository<SimulationCategory>, ISimulationCategoryRepository
    {
        public async Task<List<SimulationCategory>> GetAllAsync()
        {
            return await _context.SimulationCategories
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SimulationCategory?> GetByIdAsync(Guid id)
        {
            return await _context.SimulationCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<SimulationCategory?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.SimulationCategories
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task AddAsync(SimulationCategory entity)
        {
            await _context.SimulationCategories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SimulationCategory entity)
        {
            await _context.SaveChangesAsync();
        }
        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.SimulationCategories
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.SimulationCategories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            _context.SimulationCategories.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}