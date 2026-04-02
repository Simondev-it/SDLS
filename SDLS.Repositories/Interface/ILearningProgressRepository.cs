using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ILearningProgressRepository
    {
        Task<LearningProgress?> GetByIdAsync(Guid id, string? role = null);
        Task<List<LearningProgress>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionId = null,
            int? status = null,
            string? role = null);

        Task AddAsync(LearningProgress entity);
        Task UpdateAsync(LearningProgress entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);

        Task<List<LearningProgress>> GetByUserAndQuestionAsync(
            Guid? userId,
            Guid? questionId,
            int? status = null,
            string? role = null);
    }
}
