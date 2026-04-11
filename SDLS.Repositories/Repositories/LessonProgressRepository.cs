using Microsoft.EntityFrameworkCore;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class LessonProgressRepository : GenericRepository<LessonProgress>, ILessonProgressRepository
    {
        public async Task<List<LessonProgress>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionLessonId = null,
            int? status = null,
            string? role = null)
        {
            var query = _context.LessonProgresses
                .Include(x => x.QuestionLesson)
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (questionLessonId.HasValue)
                query = query.Where(x => x.QuestionLessonId == questionLessonId.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
                query = query.Where(x => x.QuestionLesson == null || x.QuestionLesson.Status != 0);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<LessonProgress?> GetByIdAsync(Guid id, string? role = null)
        {
            var query = _context.LessonProgresses
                .Include(x => x.QuestionLesson)
                .Where(x => x.Id == id)
                .ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
                query = query.Where(x => x.QuestionLesson == null || x.QuestionLesson.Status != 0);

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<LessonProgress?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.LessonProgresses
                .Include(x => x.QuestionLesson)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<LessonProgress>> GetByUserAndQuestionLessonAsync(
            Guid? userId,
            Guid? questionLessonId,
            int? status = null,
            string? role = null)
        {
            var query = _context.LessonProgresses
                .Include(x => x.QuestionLesson)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (questionLessonId.HasValue)
                query = query.Where(x => x.QuestionLessonId == questionLessonId.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
                query = query.Where(x => x.QuestionLesson == null || x.QuestionLesson.Status != 0);

            return await query.AsNoTracking().ToListAsync();
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
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
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