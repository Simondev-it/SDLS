using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SimulationScenarioRepository : GenericRepository<SimulationScenario>, ISimulationScenarioRepository
    {
        public async Task<IEnumerable<SimulationScenario>> GetAllAsync()
        {
            return await _context.SimulationScenarios
                .Include(x => x.SimulationDifficultyLevel)
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SimulationScenario?> GetByIdAsync(Guid id)
        {
            return await _context.SimulationScenarios
                .Include(x => x.SimulationCategory)
                .Include(x => x.SimulationChapter)
                .Include(x => x.SimulationDifficultyLevel)
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<SimulationScenario?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.SimulationScenarios
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task AddAsync(SimulationScenario entity)
        {
            await _context.SimulationScenarios.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SimulationScenario entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await DeleteSoftAsync(id);
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.SimulationScenarios
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.SimulationScenarios
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            _context.SimulationScenarios.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}