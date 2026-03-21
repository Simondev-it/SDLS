using Microsoft.EntityFrameworkCore.Storage;
using SDLS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Repositories.Interface
{
    public interface IQuestionRepository
    {
        Task<Question> GetByIdAsync(Guid id);
        Task<Question> GetByIdForUpdateAsync(Guid id);
        Task<IEnumerable<Question>> GetAllAsync();
        //Task<List<Question>> GetAllOrderedAsync();
        Task AddAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteAsync(Guid id);
        Task<Question?> GetChildQuestionAsync(Guid parentId);
        Task<List<Question>> GetAllByLessonAsync(Guid lessonId);
        Task<Question?> GetByIdWithLinksAsync(Guid id);
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
            string? searchContent = null);
    }
}
