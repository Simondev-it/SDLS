using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISavedQuestionRepository
    {
        Task<List<SavedQuestion>> GetAllAsync();
        Task<SavedQuestion?> GetByIdAsync(Guid id);
        Task<List<SavedQuestion>> GetByUserAndQuestionAsync(Guid? userId, Guid? questionId);
        Task AddAsync(SavedQuestion entity);
        Task UpdateAsync(SavedQuestion entity);
        Task DeleteAsync(Guid id);
    }
}