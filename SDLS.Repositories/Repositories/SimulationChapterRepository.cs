using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SimulationChapterRepository : GenericRepository<SimulationChapter>, ISimulationChapterRepository
    {
        public async Task<List<SimulationChapter>> GetAllAsync()
        {
            return await _context.SimulationChapters
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SimulationChapter?> GetByIdAsync(Guid id)
        {
            return await _context.SimulationChapters
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<SimulationChapter?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.SimulationChapters
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task AddAsync(SimulationChapter entity)
        {
            await _context.SimulationChapters.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SimulationChapter entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _context.SimulationChapters
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }
    }
}