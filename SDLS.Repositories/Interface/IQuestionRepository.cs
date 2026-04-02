using Microsoft.EntityFrameworkCore.Storage;
using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IQuestionRepository
    {
        Task<Question?> GetByIdAsync(Guid id, string? role = null);
        Task<Question?> GetByIdForUpdateAsync(Guid id);
        Task<IEnumerable<Question>> GetAllAsync(
            int? status = null,
            string? role = null);

        Task AddAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);

        Task<Question?> GetChildQuestionAsync(Guid parentId);
        Task<List<Question>> GetAllByLessonAsync(Guid lessonId);
        Task DeleteAnswersByQuestionIdAsync(Guid questionId);
        void RemoveQuestionTags(IEnumerable<QuestionTag> questionTags);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task UpdateParentIdAsync(Guid questionId, Guid? newParentId);
        Task<List<Question>> GetLessonQuestionsForReorderAsync(Guid lessonId);
        Task<List<Question>> GetFilteredForListAsync(
            Guid? lessonId = null,
            Guid? topicId = null,
            Guid? questionCategoryId = null,
            List<Guid>? tagIds = null,
            string? searchContent = null,
            int? status = null,
            string? role = null);
    }
}
