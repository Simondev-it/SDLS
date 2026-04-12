using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Model.Helpers;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class QuestionLessonRepository : GenericRepository<QuestionLesson>, IQuestionLessonRepository
    {
        public async Task<IEnumerable<QuestionLesson>> GetAllAsync(
            Guid? id = null,
            Guid? questionChapterId = null,
            string? name = null,
            string? description = null,
            string? content = null,
            int? status = null,
            string? role = null)
        {
            var query = _context.QuestionLessons
                .Include(x => x.QuestionChapter)
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(x => x.Id == id.Value);

            if (questionChapterId.HasValue)
                query = query.Where(x => x.QuestionChapterId == questionChapterId.Value);

            if (!string.IsNullOrWhiteSpace(name))
            {
                var keyword = name.Trim();
                query = query.Where(x => x.Name != null && EF.Functions.ILike(x.Name, $"%{keyword}%"));
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var keyword = description.Trim();
                query = query.Where(x => x.Description != null && EF.Functions.ILike(x.Description, $"%{keyword}%"));
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                var keyword = content.Trim();
                query = query.Where(x => x.Content != null && EF.Functions.ILike(x.Content, $"%{keyword}%"));
            }

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
                query = query.Where(x => x.QuestionChapter == null || x.QuestionChapter.Status != 0);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<QuestionLesson?> GetByIdAsync(Guid id, string? role = null)
        {
            var query = _context.QuestionLessons
                .Include(x => x.QuestionChapter)
                .Where(x => x.Id == id)
                .ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
                query = query.Where(x => x.QuestionChapter == null || x.QuestionChapter.Status != 0);

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<QuestionLesson?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.QuestionLessons
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(QuestionLesson lesson)
        {
            await _context.QuestionLessons.AddAsync(lesson);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(QuestionLesson lesson)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var lesson = await _context.QuestionLessons
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (lesson == null)
                return;

            lesson.Status = 0;
            lesson.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var lesson = await _context.QuestionLessons
                .FirstOrDefaultAsync(x => x.Id == id);

            if (lesson == null)
                return;

            _context.QuestionLessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LessonImage>> GetLessonImagesByLessonIdsAsync(List<Guid> lessonIds, string? role = null)
        {
            if (lessonIds == null || lessonIds.Count == 0)
                return new List<LessonImage>();

            var query = _context.LessonImages
                .Where(x => lessonIds.Contains(x.QuestionLessonId))
                .ApplyRoleFilter(role);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<List<LessonImage>> GetLessonImagesByLessonIdForUpdateAsync(Guid lessonId)
        {
            return await _context.LessonImages
                .Where(x => x.QuestionLessonId == lessonId)
                .ToListAsync();
        }

        public void RemoveLessonImages(IEnumerable<LessonImage> images)
        {
            _context.LessonImages.RemoveRange(images);
        }

        public void AddLessonImages(IEnumerable<LessonImage> images)
        {
            _context.LessonImages.AddRange(images);
        }

        public async Task SoftDeleteLessonImagesAsync(Guid lessonId, DateTime now)
        {
            await _context.LessonImages
                .Where(x => x.QuestionLessonId == lessonId && x.Status == 1)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Status, 0)
                    .SetProperty(x => x.UpdateAt, now));
        }

        public async Task RestoreLessonImagesAsync(Guid lessonId, DateTime now)
        {
            await _context.LessonImages
                .Where(x => x.QuestionLessonId == lessonId && x.Status == 0)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Status, 1)
                    .SetProperty(x => x.UpdateAt, now));
        }
    }
}