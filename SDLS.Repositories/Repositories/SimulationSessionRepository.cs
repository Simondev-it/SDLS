using Microsoft.EntityFrameworkCore;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SimulationSessionRepository : GenericRepository<SimulationSession>, ISimulationSessionRepository
    {
        public async Task<List<SimulationSession>> GetAllAsync(
            Guid? id = null,
            Guid? situationExamId = null,
            Guid? userId = null,
            int? status = null,
            string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<SimulationSession> query = isPrivileged
                ? _context.SimulationSessions
                    .Include(x => x.SituationExam)
                    .Include(x => x.SimulationSessionDetails)
                : _context.SimulationSessions
                    .Include(x => x.SituationExam)
                    .Include(x => x.SimulationSessionDetails.Where(d => d.Status != 0));

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (situationExamId.HasValue)
                query = query.Where(x => x.SituationExamId == situationExamId.Value);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!isPrivileged)
                query = query.Where(x => x.SituationExam == null || x.SituationExam.Status != 0);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<SimulationSession?> GetByIdAsync(Guid id, string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<SimulationSession> query = isPrivileged
                ? _context.SimulationSessions
                    .Include(x => x.SituationExam)
                    .Include(x => x.SimulationSessionDetails)
                : _context.SimulationSessions
                    .Include(x => x.SituationExam)
                    .Include(x => x.SimulationSessionDetails.Where(d => d.Status != 0));

            query = query.Where(x => x.Id == id)
                         .ApplyRoleFilter(role);

            if (!isPrivileged)
                query = query.Where(x => x.SituationExam == null || x.SituationExam.Status != 0);

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<SimulationSession?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.SimulationSessions
                .Include(x => x.SimulationSessionDetails)
                .FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.SimulationSessions
                .Include(x => x.SimulationSessionDetails)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            var now = DateTimeHelper.GetVietnamNow();
            existing.Status = 0;
            existing.UpdateAt = now;

            foreach (var detail in existing.SimulationSessionDetails.Where(x => x.Status == 1))
            {
                detail.Status = 0;
                detail.UpdateAt = now;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.SimulationSessions
                .Include(x => x.SimulationSessionDetails)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            if (existing.SimulationSessionDetails.Any())
                _context.SimulationSessionDetails.RemoveRange(existing.SimulationSessionDetails);

            _context.SimulationSessions.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}