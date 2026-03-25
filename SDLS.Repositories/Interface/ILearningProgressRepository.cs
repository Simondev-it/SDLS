using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ILearningProgressRepository
    {
        Task<LearningProgress?> GetByIdAsync(Guid id);
        Task<List<LearningProgress>> GetAllAsync();
        Task AddAsync(LearningProgress entity);
        Task UpdateAsync(LearningProgress entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);

        Task<List<LearningProgress>> GetByUserAndQuestionAsync(Guid? userId, Guid? questionId);
    }
}
