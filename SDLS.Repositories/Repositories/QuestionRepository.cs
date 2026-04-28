using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;
using SDLS.Repositories.Helper;
using System.Globalization;
using System.Text;

namespace SDLS.Repositories.Repositories
{
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        private readonly Guid CREATE_TAG_ID = Guid.Parse("763a5be4-963a-487d-a3b4-6a826026c94e");
        private readonly Guid UPDATE_TAG_ID = Guid.Parse("8317546b-0cc6-43e9-a917-0ae9d090ec16");

        public async Task<Question?> GetByIdAsync(Guid id, string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<Question> query = isPrivileged
                ? _context.Questions
                    .Include(q => q.Answers)
                    .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
                    .Include(q => q.QuestionLesson).ThenInclude(ql => ql.QuestionChapter)
                    .Include(q => q.QuestionTopic)
                    .Include(q => q.QuestionCategory)
                : _context.Questions
                    .Include(q => q.Answers.Where(a => a.Status != 0))
                    .Include(q => q.QuestionTags.Where(qt => qt.Status != 0)).ThenInclude(qt => qt.Tag)
                    .Include(q => q.QuestionLesson).ThenInclude(ql => ql.QuestionChapter)
                    .Include(q => q.QuestionTopic)
                    .Include(q => q.QuestionCategory);

            query = query.Where(q => q.Id == id)
                         .ApplyRoleFilter(role);

            if (!isPrivileged)
            {
                query = query.Where(q =>
                    (q.QuestionLesson == null || q.QuestionLesson.Status != 0) &&
                    (q.QuestionTopic == null || q.QuestionTopic.Status != 0) &&
                    (q.QuestionCategory == null || q.QuestionCategory.Status != 0));
            }

            return await query.AsNoTracking().FirstOrDefaultAsync();
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
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Question>> GetAllAsync(int? status = null, string? role = null)
        {
            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<Question> query = isPrivileged
                ? _context.Questions
                    .Include(q => q.Answers)
                    .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
                    .Include(q => q.QuestionLesson).ThenInclude(ql => ql.QuestionChapter)
                    .Include(q => q.QuestionTopic)
                    .Include(q => q.QuestionCategory)
                    .Include(q => q.InverseParent)
                : _context.Questions
                    .Include(q => q.Answers.Where(a => a.Status != 0))
                    .Include(q => q.QuestionTags.Where(qt => qt.Status != 0))
                    .Include(q => q.QuestionLesson).ThenInclude(ql => ql.QuestionChapter)
                    .Include(q => q.QuestionTopic)
                    .Include(q => q.QuestionCategory)
                    .Include(q => q.InverseParent.Where(ip => ip.Status != 0));

            if (status.HasValue)
                query = query.Where(q => q.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!isPrivileged)
            {
                query = query.Where(q =>
                    (q.QuestionLesson == null || q.QuestionLesson.Status != 0) &&
                    (q.QuestionTopic == null || q.QuestionTopic.Status != 0) &&
                    (q.QuestionCategory == null || q.QuestionCategory.Status != 0));
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<Question?> GetByIdForAdminAsync(Guid id)
        {
            var question = await _context.Questions
                .Include(q => q.Answers)
                    .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
                    .Include(q => q.QuestionLesson).ThenInclude(ql => ql.QuestionChapter)
                    .Include(q => q.QuestionTopic)
                    .Include(q => q.QuestionCategory)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question != null)
            {
                // Xóa cả 2 tag đặc biệt nếu tồn tại
                var specialTags = question.QuestionTags
                    .Where(qt => qt.TagId == CREATE_TAG_ID || qt.TagId == UPDATE_TAG_ID)
                    .ToList();

                if (specialTags.Any())
                {
                    _context.QuestionTags.RemoveRange(specialTags);
                    await _context.SaveChangesAsync();
                }
            }
            return question;
        }

        public async Task AddAsync(Question question)
        {
            // Đảm bảo QuestionTags không bị null
            question.QuestionTags ??= new List<QuestionTag>();

            // Luôn thêm Tag cho lúc Create
            if (!question.QuestionTags.Any(t => t.TagId == CREATE_TAG_ID))
            {
                question.QuestionTags.Add(new QuestionTag
                {
                    TagId = CREATE_TAG_ID,
                    QuestionId = question.Id,
                    Status = 1,
                    CreateAt = DateTimeHelper.GetVietnamNow()
                });
            }

            await this.CreateAsync(question);
        }

        public async Task UpdateAsync(Question question)
        {
            var currentTagsInDb = await _context.QuestionTags
                .Where(qt => qt.QuestionId == question.Id)
                .ToListAsync();

            var specialTags = currentTagsInDb
                .Where(qt => qt.TagId == CREATE_TAG_ID || qt.TagId == UPDATE_TAG_ID)
                .ToList();

            if (specialTags.Any())
            {
                _context.QuestionTags.RemoveRange(specialTags);
            }

            if (question.QuestionTags != null)
            {
                var createTagInList = question.QuestionTags.FirstOrDefault(t => t.TagId == CREATE_TAG_ID);
                if (createTagInList != null)
                {
                    question.QuestionTags.Remove(createTagInList);
                }

                if (!question.QuestionTags.Any(t => t.TagId == UPDATE_TAG_ID))
                {
                    question.QuestionTags.Add(new QuestionTag
                    {
                        TagId = UPDATE_TAG_ID,
                        QuestionId = question.Id,
                        Status = 1,
                        CreateAt = DateTimeHelper.GetVietnamNow()
                    });
                }
            }

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

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id && (x.Status == 1 || x.Status == 0));
            if (existing == null)
                return;

            existing.Status = existing.Status == 0 ? 1 : 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
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
                    .SetProperty(x => x.UpdateAt, DateTimeHelper.GetVietnamNow()));

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
            return this.GetById(parentId)?.InverseParent.FirstOrDefault();
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
                .Where(q => q.QuestionLessonId == lessonId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateParentIdAsync(Guid questionId, Guid? newParentId)
        {
            await _context.Questions
                .Where(q => q.Id == questionId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(q => q.ParentId, newParentId)
                    .SetProperty(q => q.UpdateAt, DateTimeHelper.GetVietnamNow()));
        }

        public async Task<List<Question>> GetLessonQuestionsForReorderAsync(Guid lessonId)
        {
            return await _context.Questions
                .Where(q => q.QuestionLessonId == lessonId)
                .Select(q => new Question { Id = q.Id, ParentId = q.ParentId })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Question>> GetFilteredForListAsync(
            Guid? lessonId = null,
            Guid? topicId = null,
            Guid? questionCategoryId = null,
            List<Guid>? tagIds = null,
            string? searchContent = null,
            int? status = null,
            string? role = null)
        {
            var normalizedTagIds = tagIds?
                .Where(x => x != Guid.Empty)
                .Distinct()
                .ToList() ?? new List<Guid>();

            var isPrivileged = QueryableRoleFilterExtensions.IsPrivilegedRole(role);

            IQueryable<Question> query = _context.Questions;

            if (lessonId.HasValue)
                query = query.Where(q => q.QuestionLessonId == lessonId.Value);

            if (topicId.HasValue)
                query = query.Where(q => q.QuestionTopicId == topicId.Value);

            if (questionCategoryId.HasValue)
                query = query.Where(q => q.QuestionCategoryId == questionCategoryId.Value);

            if (status.HasValue)
                query = query.Where(q => q.Status == status.Value);

            if (normalizedTagIds.Count > 0)
            {
                var requiredTagCount = normalizedTagIds.Count;

                query = query.Where(q =>
                    q.QuestionTags
                        .Where(qt => normalizedTagIds.Contains(qt.TagId))
                        .Select(qt => qt.TagId)
                        .Distinct()
                        .Count() == requiredTagCount);
            }

            query = query.ApplyRoleFilter(role);

            if (!isPrivileged)
            {
                query = query.Where(q =>
                    (q.QuestionLesson == null || q.QuestionLesson.Status != 0) &&
                    (q.QuestionTopic == null || q.QuestionTopic.Status != 0) &&
                    (q.QuestionCategory == null || q.QuestionCategory.Status != 0));
            }

            IQueryable<Question> includeQuery;

            if (isPrivileged)
            {
                includeQuery = query
                    .Include(q => q.Answers)
                    .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
                    .Include(q => q.QuestionLesson).ThenInclude(ql => ql.QuestionChapter)
                    .Include(q => q.QuestionTopic)
                    .Include(q => q.QuestionCategory);
            }
            else
            {
                includeQuery = query
                    .Include(q => q.Answers.Where(a => a.Status != 0))
                    .Include(q => q.QuestionTags.Where(qt => qt.Status != 0)).ThenInclude(qt => qt.Tag)
                    .Include(q => q.QuestionLesson).ThenInclude(ql => ql.QuestionChapter)
                    .Include(q => q.QuestionTopic)
                    .Include(q => q.QuestionCategory);
            }

            var list = await includeQuery
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
