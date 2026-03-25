using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IQuestionCategoryRepository
    {
        Task<List<QuestionCategory>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? role = null);

        Task<QuestionCategory?> GetByIdAsync(Guid id, string? role = null);
        Task<QuestionCategory?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(QuestionCategory entity);
        Task UpdateAsync(QuestionCategory entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}