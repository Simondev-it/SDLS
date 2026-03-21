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
            int? status = 1,
            int page = 1,
            int pageSize = 20);

        Task<QuestionLessonDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(QuestionLessonCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, QuestionLessonUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}