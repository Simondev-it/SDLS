using SDLS.Model.DTOs;
using SDLS.Model.DTOs.QuestionLesson;

namespace SDLS.Services.Interfaces
{
    public interface IQuestionLessonService
    {
        Task<PagedResult<QuestionLessonDTO>> GetAllAsync(
            Guid? id = null,
            Guid? questionChapterId = null,
            string? name = null,
            string? description = null,
            string? content = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<QuestionLessonDTO> GetByIdAsync(Guid id);
        Task<QuestionLessonDTO> CreateAsync(QuestionLessonCreateDTO dto);
        Task<QuestionLessonDTO> UpdateAsync(Guid id, QuestionLessonUpdateDTO dto);

        Task<QuestionLessonDTO> DeleteAsync(Guid id);
        Task<QuestionLessonDTO> DeleteSoftAsync(Guid id);
        Task<QuestionLessonDTO> DeleteHardAsync(Guid id);
    }
}