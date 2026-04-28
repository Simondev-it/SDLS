using Microsoft.EntityFrameworkCore;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SituationExamRepository : GenericRepository<SituationExam>, ISituationExamRepository
    {
        public async Task<List<SituationExam>> GetAllAsync(
            Guid? id = null,
            string? title = null,
            string? description = null,
            bool? isRandom = null,
            int? status = null,
            string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<SituationExam> query = isPrivileged
                ? _context.SituationExams
                    .Include(x => x.SimulationExams)
                        .ThenInclude(se => se.Simulation)
                            .ThenInclude(s => s.SimulationCategory)
                    .Include(x => x.SimulationExams)
                        .ThenInclude(se => se.Simulation)
                            .ThenInclude(s => s.SimulationChapter)
                    .Include(x => x.SimulationExams)
                        .ThenInclude(se => se.Simulation)
                            .ThenInclude(s => s.SimulationDifficultyLevel).Where(sie => sie.Status != 2)
                : _context.SituationExams
                    .Include(x => x.SimulationExams.Where(se => se.Status != 0))
                        .ThenInclude(se => se.Simulation)
                            .ThenInclude(s => s.SimulationCategory)
                    .Include(x => x.SimulationExams.Where(se => se.Status != 0))
                        .ThenInclude(se => se.Simulation)
                            .ThenInclude(s => s.SimulationChapter)
                    .Include(x => x.SimulationExams.Where(se => se.Status != 0))
                        .ThenInclude(se => se.Simulation)
                            .ThenInclude(s => s.SimulationDifficultyLevel).Where(sie => sie.Status != 0);

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (!string.IsNullOrWhiteSpace(title))
            {
                var keyword = title.Trim();
                query = query.Where(x => x.Title != null && EF.Functions.ILike(x.Title, $"%{keyword}%"));
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var keyword = description.Trim();
                query = query.Where(x => x.Description != null && EF.Functions.ILike(x.Description, $"%{keyword}%"));
            }

            if (isRandom.HasValue)
                query = query.Where(x => x.IsRandom == isRandom.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!isPrivileged)
            {
                query = query.Where(x =>
                    x.SimulationExams.All(se => se.Simulation == null || se.Simulation.Status != 0));
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<SituationExam?> GetByIdAsync(Guid id, string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<SituationExam> query = isPrivileged
                ? _context.SituationExams
                    .Include(x => x.SimulationExams)
                        .ThenInclude(se => se.Simulation)
                            .ThenInclude(s => s.SimulationCategory)
                    .Include(x => x.SimulationExams)
                        .ThenInclude(se => se.Simulation)
                            .ThenInclude(s => s.SimulationChapter)
                    .Include(x => x.SimulationExams)
                        .ThenInclude(se => se.Simulation)
                            .ThenInclude(s => s.SimulationDifficultyLevel)
                : _context.SituationExams
                    .Include(x => x.SimulationExams.Where(se => se.Status != 0))
                        .ThenInclude(se => se.Simulation)
                            .ThenInclude(s => s.SimulationCategory)
                    .Include(x => x.SimulationExams.Where(se => se.Status != 0))
                        .ThenInclude(se => se.Simulation)
                            .ThenInclude(s => s.SimulationChapter)
                    .Include(x => x.SimulationExams.Where(se => se.Status != 0))
                        .ThenInclude(se => se.Simulation)
                            .ThenInclude(s => s.SimulationDifficultyLevel);

            query = query.Where(x => x.Id == id)
                         .ApplyRoleFilter(role);

            if (!isPrivileged)
            {
                query = query.Where(x =>
                    x.SimulationExams.All(se => se.Simulation == null || se.Simulation.Status != 0));
            }

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<SituationExam?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.SituationExams
                .Include(x => x.SimulationExams)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(SituationExam entity)
        {
            await _context.SituationExams.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SituationExam entity)
        {
            await _context.SaveChangesAsync();
        }
        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.SituationExams
                .Include(x => x.SimulationExams)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            var now = DateTimeHelper.GetVietnamNow();
            existing.Status = 0;
            existing.UpdateAt = now;

            foreach (var child in existing.SimulationExams.Where(x => x.Status == 1))
            {
                child.Status = 0;
                child.UpdateAt = now;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.SituationExams
                .Include(x => x.SimulationExams)
                    .ThenInclude(se => se.SimulationSessionDetails)
                .Include(x => x.SimulationSessions)
                    .ThenInclude(ss => ss.SimulationSessionDetails)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            var detailsFromSimulationExams = existing.SimulationExams
                .SelectMany(x => x.SimulationSessionDetails)
                .ToList();

            if (detailsFromSimulationExams.Any())
                _context.SimulationSessionDetails.RemoveRange(detailsFromSimulationExams);

            if (existing.SimulationExams.Any())
                _context.SimulationExams.RemoveRange(existing.SimulationExams);

            var detailsFromSessions = existing.SimulationSessions
                .SelectMany(x => x.SimulationSessionDetails)
                .ToList();

            if (detailsFromSessions.Any())
                _context.SimulationSessionDetails.RemoveRange(detailsFromSessions);

            if (existing.SimulationSessions.Any())
                _context.SimulationSessions.RemoveRange(existing.SimulationSessions);

            _context.SituationExams.Remove(existing);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetPassScoreAsync(Guid situationExamId)
        {
            var exam = await _context.SituationExams
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == situationExamId && x.Status == 1);

            if (exam == null)
                throw new KeyNotFoundException("Không tìm thấy SituationExam.");

            return exam.PassScore ?? 0;
        }
    }
}