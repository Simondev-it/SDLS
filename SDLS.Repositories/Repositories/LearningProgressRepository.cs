using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class LearningProgressRepository : GenericRepository<LearningProgress>, ILearningProgressRepository
    {
        public async Task<LearningProgress?> GetByIdAsync(Guid id)
        {
            return await _context.LearningProgresses
                .Include(lp => lp.Question)
                .Include(lp => lp.User)
                .FirstOrDefaultAsync(lp => lp.Id == id && lp.Status == 1);
        }

        public async Task<List<LearningProgress>> GetAllAsync()
        {
            return await _context.LearningProgresses
                .Include(lp => lp.Question)
                .Include(lp => lp.User)
                .Where(lp => lp.Status == 1)
                .ToListAsync();
        }

        public async Task AddAsync(LearningProgress entity)
        {
            await _context.LearningProgresses.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LearningProgress entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.LearningProgresses
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.LearningProgresses
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            _context.LearningProgresses.Remove(existing);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LearningProgress>> GetByUserAndQuestionAsync(Guid? userId, Guid? questionId)
        {
            var query = _context.LearningProgresses
                .Include(lp => lp.Question)
                .Include(lp => lp.User)
                .Where(lp => lp.Status == 1);

            if (userId.HasValue)
                query = query.Where(lp => lp.UserId == userId.Value);

            if (questionId.HasValue)
                query = query.Where(lp => lp.QuestionId == questionId.Value);

            return await query.ToListAsync();
        }
    }
}
