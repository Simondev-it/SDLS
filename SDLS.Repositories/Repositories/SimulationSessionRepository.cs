using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SimulationSessionRepository : GenericRepository<SimulationSession>, ISimulationSessionRepository
    {
        public async Task<List<SimulationSession>> GetAllAsync(
            Guid? id = null,
            Guid? situationExamId = null,
            Guid? userId = null)
        {
            var query = _context.SimulationSessions
                .Include(x => x.SimulationSessionDetails.Where(d => d.Status == 1))
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (situationExamId.HasValue)
                query = query.Where(x => x.SituationExamId == situationExamId.Value);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            return await query.ToListAsync();
        }

        public async Task<SimulationSession?> GetByIdAsync(Guid id)
        {
            return await _context.SimulationSessions
                .Include(x => x.SituationExam)
                .Include(x => x.SimulationSessionDetails.Where(d => d.Status == 1))
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<SimulationSession?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.SimulationSessions
                .Include(x => x.SimulationSessionDetails)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task AddAsync(SimulationSession entity)
        {
            await _context.SimulationSessions.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SimulationSession entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _context.SimulationSessions
                .Include(x => x.SimulationSessionDetails)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            var now = DateTime.UtcNow.ToLocalTime();
            existing.Status = 0;
            existing.UpdateAt = now;

            foreach (var detail in existing.SimulationSessionDetails.Where(x => x.Status == 1))
            {
                detail.Status = 0;
                detail.UpdateAt = now;
            }

            await _context.SaveChangesAsync();
        }
    }
}