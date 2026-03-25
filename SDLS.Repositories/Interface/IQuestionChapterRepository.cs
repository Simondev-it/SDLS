using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IQuestionChapterRepository
    {
        Task<IEnumerable<QuestionChapter>> GetAllAsync();
        Task<QuestionChapter?> GetByIdAsync(Guid id);
        Task<QuestionChapter?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(QuestionChapter chapter);
        Task UpdateAsync(QuestionChapter chapter);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}