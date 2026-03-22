using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IQuestionTopicRepository
    {
        Task<List<QuestionTopic>> GetAllAsync();
        Task<QuestionTopic?> GetByIdAsync(Guid id);
        Task<QuestionTopic?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(QuestionTopic entity);
        Task UpdateAsync(QuestionTopic entity);
        Task DeleteAsync(Guid id);
    }
}