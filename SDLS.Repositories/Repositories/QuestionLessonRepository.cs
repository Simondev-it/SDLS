using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
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
            int? status = 1)
        {
            var query = _context.QuestionLessons
                .Include(x => x.QuestionChapter)
                .AsNoTracking()
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

            return await query.ToListAsync();
        }

        public async Task<QuestionLesson?> GetByIdAsync(Guid id)
        {
            return await _context.QuestionLessons
                .Include(x => x.QuestionChapter)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<QuestionLesson?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.QuestionLessons
                .Include(x => x.QuestionChapter)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
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
            lesson.UpdateAt = DateTime.UtcNow.ToLocalTime();
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

        public async Task<List<LessonImage>> GetLessonImagesByLessonIdsAsync(List<Guid> lessonIds)
        {
            if (lessonIds == null || lessonIds.Count == 0)
                return new List<LessonImage>();

            return await _context.LessonImages
                .Where(x => lessonIds.Contains(x.QuestionLessonId) && x.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<LessonImage>> GetLessonImagesByLessonIdForUpdateAsync(Guid lessonId)
        {
            return await _context.LessonImages
                .Where(x => x.QuestionLessonId == lessonId && x.Status == 1)
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
    }
}