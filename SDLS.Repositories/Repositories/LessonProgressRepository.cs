using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class LessonProgressRepository : GenericRepository<LessonProgress>, ILessonProgressRepository
    {
        public async Task<List<LessonProgress>> GetAllAsync()
        {
            return await _context.LessonProgresses
                .Include(x => x.QuestionLesson)
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LessonProgress?> GetByIdAsync(Guid id)
        {
            return await _context.LessonProgresses
                .Include(x => x.QuestionLesson)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<LessonProgress?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.LessonProgresses
                .Include(x => x.QuestionLesson)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<List<LessonProgress>> GetByUserAndQuestionLessonAsync(Guid? userId, Guid? questionLessonId)
        {
            var query = _context.LessonProgresses
                .Include(x => x.QuestionLesson)
                .Where(x => x.Status == 1);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (questionLessonId.HasValue)
                query = query.Where(x => x.QuestionLessonId == questionLessonId.Value);

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(LessonProgress entity)
        {
            await _context.LessonProgresses.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LessonProgress entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.LessonProgresses
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var entity = await _context.LessonProgresses
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            _context.LessonProgresses.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}