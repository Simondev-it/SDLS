using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IQuestionTopicRepository
    {
        Task<List<QuestionTopic>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? role = null);

        Task<QuestionTopic?> GetByIdAsync(Guid id, string? role = null);
        Task<QuestionTopic?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(QuestionTopic entity);
        Task UpdateAsync(QuestionTopic entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}