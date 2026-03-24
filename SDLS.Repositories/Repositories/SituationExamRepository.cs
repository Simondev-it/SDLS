using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SituationExamRepository : GenericRepository<SituationExam>, ISituationExamRepository
    {
        public async Task<List<SituationExam>> GetAllAsync(
            Guid? id = null,
            string? title = null,
            string? description = null,
            bool? isRandom = null)
        {
            var query = _context.SituationExams
                .Include(x => x.SimulationExams.Where(se => se.Status == 1))
                    .ThenInclude(se => se.Simulation)
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .AsQueryable();

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

            return await query.ToListAsync();
        }

        public async Task<SituationExam?> GetByIdAsync(Guid id)
        {
            return await _context.SituationExams
                .Include(x => x.SimulationExams.Where(se => se.Status == 1))
                    .ThenInclude(se => se.Simulation)
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<SituationExam?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.SituationExams
                .Include(x => x.SimulationExams)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
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

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _context.SituationExams
                .Include(x => x.SimulationExams)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            var now = DateTime.UtcNow.ToLocalTime();
            existing.Status = 0;
            existing.UpdateAt = now;

            foreach (var child in existing.SimulationExams.Where(x => x.Status == 1))
            {
                child.Status = 0;
                child.UpdateAt = now;
            }

            await _context.SaveChangesAsync();
        }
    }
}