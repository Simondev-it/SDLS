using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IQuestionCategoryRepository
    {
        Task<List<QuestionCategory>> GetAllAsync();
        Task<QuestionCategory?> GetByIdAsync(Guid id);
        Task<QuestionCategory?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(QuestionCategory entity);
        Task UpdateAsync(QuestionCategory entity);

        Task DeleteAsync(Guid id);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}