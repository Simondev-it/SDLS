using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISavedQuestionRepository
    {
        Task<List<SavedQuestion>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionId = null,
            int? status = null,
            string? role = null);

        Task<SavedQuestion?> GetByIdAsync(Guid id, string? role = null);
        Task<List<SavedQuestion>> GetByUserAndQuestionAsync(Guid? userId, Guid? questionId);
        Task AddAsync(SavedQuestion entity);
        Task UpdateAsync(SavedQuestion entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}