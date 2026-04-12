using SDLS.Model.DTOs;
using SDLS.Model.DTOs.LearningProgress;

namespace SDLS.Services.Interfaces
{
    public interface ILearningProgressService
    {
        Task<IEnumerable<LearningProgressDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionId = null,
            int? status = null);

        Task<PagedResult<LearningProgressDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<LearningProgressDTO?> GetByIdAsync(Guid id);
        Task<LearningProgressDTO> CreateAsync(LearningProgressCreateDTO dto);
        Task<LearningProgressDTO> UpdateAsync(Guid id, LearningProgressUpdateDTO dto);
        Task<LearningProgressDTO> DeleteSoftAsync(Guid id);
        Task<LearningProgressDTO> DeleteHardAsync(Guid id);

        Task<List<LearningProgressDTO>> GetByUserAndQuestionAsync(
            Guid? userId,
            Guid? questionId,
            int? status = null);
    }
}
