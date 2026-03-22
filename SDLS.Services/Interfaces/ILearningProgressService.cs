using SDLS.Model.DTOs.LearningProgress;

namespace SDLS.Services.Interfaces
{
    public interface ILearningProgressService
    {
        Task<IEnumerable<LearningProgressDTO>> GetAllAsync();
        Task<LearningProgressDTO?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(LearningProgressCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, LearningProgressUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
        Task<List<LearningProgressDTO>> GetByUserAndQuestionAsync(Guid? userId, Guid? questionId);
    }
}
