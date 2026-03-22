using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SimulationDifficultyLevelRepository : GenericRepository<SimulationDifficultyLevel>, ISimulationDifficultyLevelRepository
    {
        public async Task<List<SimulationDifficultyLevel>> GetAllAsync()
        {
            return await _context.SimulationDifficultyLevels
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SimulationDifficultyLevel?> GetByIdAsync(Guid id)
        {
            return await _context.SimulationDifficultyLevels
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<SimulationDifficultyLevel?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.SimulationDifficultyLevels
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task AddAsync(SimulationDifficultyLevel entity)
        {
            await _context.SimulationDifficultyLevels.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SimulationDifficultyLevel entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _context.SimulationDifficultyLevels
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();

            await _context.SaveChangesAsync();
        }
    }
}