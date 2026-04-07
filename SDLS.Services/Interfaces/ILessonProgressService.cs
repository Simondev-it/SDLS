using SDLS.Model.DTOs;
using SDLS.Model.DTOs.LessonProgress;

namespace SDLS.Services.Interfaces
{
    public interface ILessonProgressService
    {
        Task<List<LessonProgressDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionLessonId = null,
            int? status = null);

        Task<PagedResult<LessonProgressDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionLessonId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<LessonProgressDTO?> GetByIdAsync(Guid id);
        Task<List<LessonProgressDTO>> GetByUserIdAsync(Guid userId, int? status = null);
        Task<LessonProgressDTO> CreateAsync(LessonProgressCreateDTO dto);
        Task<LessonProgressDTO> UpdateAsync(Guid id, LessonProgressUpdateDTO dto);

        Task<LessonProgressDTO> DeleteSoftAsync(Guid id);
        Task<LessonProgressDTO> DeleteHardAsync(Guid id);
    }
}