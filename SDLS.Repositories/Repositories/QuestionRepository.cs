using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;
using System.Globalization;
using System.Text;

namespace SDLS.Repositories.Repositories
{
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        public async Task<Question> GetByIdAsync(Guid id)
        {
            return await _context.Questions
                .Include(q => q.Answers.Where(a => a.Status == 1))
                .Include(q => q.QuestionTags.Where(qt => qt.Status == 1))
                .Include(q => q.QuestionLesson)
                    .ThenInclude(ql => ql.QuestionChapter)
                .Include(q => q.QuestionTopic)
                .Include(q => q.QuestionCategory)
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id && q.Status == 1);
        }

        public async Task<Question> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .Include(q => q.QuestionTags)
                .Include(q => q.QuestionLesson)
                    .ThenInclude(ql => ql.QuestionChapter)
                .Include(q => q.QuestionTopic)
                .Include(q => q.QuestionCategory)
                .FirstOrDefaultAsync(q => q.Id == id && q.Status == 1);
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _context.Questions
                .Include(q => q.Answers.Where(a => a.Status == 1))
                .Include(q => q.QuestionTags.Where(qt => qt.Status == 1))
                .Include(q => q.QuestionLesson)
                    .ThenInclude(ql => ql.QuestionChapter)
                .Include(q => q.QuestionTopic)
                .Include(q => q.QuestionCategory)
                .Include(q => q.InverseParent)
                .Where(q => q.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Question question)
        {
            this.CreateAsync(question);
        }

        public async Task UpdateAsync(Question question)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAnswersByQuestionIdAsync(Guid questionId)
        {
            var answers = await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();

            _context.Answers.RemoveRange(answers);
            await _context.SaveChangesAsync();
        }

        public async Task AddAnswerAsync(Answer answer)
        {
            _context.Answers.AddAsync(answer);
            await SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await DeleteSoftAsync(id);
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.Questions
                .Include(x => x.Answers)
                .Include(x => x.QuestionTags)
                .Include(x => x.ExamQuestions)
                .Include(x => x.LearningProgresses)
                .Include(x => x.SavedQuestions)
                .Include(x => x.Reports)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            await _context.Questions
                .Where(x => x.ParentId == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.ParentId, (Guid?)null)
                    .SetProperty(x => x.UpdateAt, DateTime.UtcNow.ToLocalTime()));

            if (existing.Answers.Any()) _context.Answers.RemoveRange(existing.Answers);
            if (existing.QuestionTags.Any()) _context.QuestionTags.RemoveRange(existing.QuestionTags);
            if (existing.ExamQuestions.Any()) _context.ExamQuestions.RemoveRange(existing.ExamQuestions);
            if (existing.LearningProgresses.Any()) _context.LearningProgresses.RemoveRange(existing.LearningProgresses);
            if (existing.SavedQuestions.Any()) _context.SavedQuestions.RemoveRange(existing.SavedQuestions);
            if (existing.Reports.Any()) _context.Reports.RemoveRange(existing.Reports);

            _context.Questions.Remove(existing);
            await _context.SaveChangesAsync();
        }

        public async Task<Question?> GetChildQuestionAsync(Guid parentId)
        {
            return this.GetById(parentId)?.InverseParent.FirstOrDefault(q => q.Status == 1);
        }

        public async Task<List<Question>> GetAllByLessonAsync(Guid lessonId)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .Include(q => q.InverseParent)
                .Include(q => q.QuestionLesson)
                    .ThenInclude(ql => ql.QuestionChapter)
                .Include(q => q.QuestionTopic)
                .Include(q => q.QuestionCategory)
                .Where(q => q.QuestionLessonId == lessonId && q.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Question?> GetByIdWithLinksAsync(Guid id)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .Include(q => q.InverseParent)
                .Include(q => q.QuestionLesson)
                    .ThenInclude(ql => ql.QuestionChapter)
                .Include(q => q.QuestionTopic)
                .Include(q => q.QuestionCategory)
                .FirstOrDefaultAsync(q => q.Id == id && q.Status == 1);
        }

        public async Task UpdateParentIdAsync(Guid questionId, Guid? newParentId)
        {
            await _context.Questions
                .Where(q => q.Id == questionId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(q => q.ParentId, newParentId)
                    .SetProperty(q => q.UpdateAt, DateTime.UtcNow.ToLocalTime()));
        }

        public async Task<List<Question>> GetLessonQuestionsForReorderAsync(Guid lessonId)
        {
            return await _context.Questions
                .Where(q => q.QuestionLessonId == lessonId && q.Status == 1)
                .Select(q => new Question { Id = q.Id, ParentId = q.ParentId })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Question>> GetFilteredForListAsync(
            Guid? lessonId = null,
            Guid? topicId = null,
            Guid? questionCategoryId = null,
            List<Guid>? tagIds = null,
            string? searchContent = null)
        {
            var normalizedTagIds = tagIds?
                .Where(x => x != Guid.Empty)
                .Distinct()
                .ToList() ?? new List<Guid>();

            var query = _context.Questions
                .Where(q => q.Status == 1);

            if (lessonId.HasValue)
                query = query.Where(q => q.QuestionLessonId == lessonId.Value);

            if (topicId.HasValue)
                query = query.Where(q => q.QuestionTopicId == topicId.Value);

            if (questionCategoryId.HasValue)
                query = query.Where(q => q.QuestionCategoryId == questionCategoryId.Value);

            if (normalizedTagIds.Count > 0)
            {
                var requiredTagCount = normalizedTagIds.Count;

                query = query.Where(q =>
                    q.QuestionTags
                        .Where(qt => qt.Status == 1 && normalizedTagIds.Contains(qt.TagId))
                        .Select(qt => qt.TagId)
                        .Distinct()
                        .Count() == requiredTagCount);
            }

            var list = await query
                .Include(q => q.Answers.Where(a => a.Status == 1))
                .Include(q => q.QuestionTags.Where(qt => qt.Status == 1))
                .Include(q => q.QuestionLesson)
                    .ThenInclude(ql => ql.QuestionChapter)
                .Include(q => q.QuestionTopic)
                .Include(q => q.QuestionCategory)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(searchContent))
            {
                list = list
                    .Where(q => ContainsNormalized(q.Content, searchContent))
                    .ToList();
            }

            return list;
        }

        private static bool ContainsNormalized(string? source, string? keyword)
        {
            var left = NormalizeText(source);
            var right = NormalizeText(keyword);

            if (string.IsNullOrWhiteSpace(right))
                return true;

            return left.Contains(right, StringComparison.Ordinal);
        }

        private static string NormalizeText(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var formD = input.Trim().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in formD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            var normalized = sb.ToString().Normalize(NormalizationForm.FormC)
                .Replace('đ', 'd')
                .Replace('Đ', 'D');

            normalized = string.Join(' ', normalized
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));

            return normalized.ToLowerInvariant();
        }

        public void RemoveQuestionTags(IEnumerable<QuestionTag> questionTags)
        {
            _context.QuestionTags.RemoveRange(questionTags);
        }
    }
}
