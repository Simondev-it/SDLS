using SDLS.Model.DTOs;
using SDLS.Model.DTOs.QuestionChapter;

namespace SDLS.Services.Interfaces
{
    public interface IQuestionChapterService
    {
        Task<PagedResult<QuestionChapterDTO>> GetAllAsync(
            Guid? id = null,
            Guid? drivingLicenseId = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<QuestionChapterDTO> GetByIdAsync(Guid id);
        Task<QuestionChapterDTO> CreateAsync(QuestionChapterCreateDTO dto);
        Task<QuestionChapterDTO> UpdateAsync(Guid id, QuestionChapterUpdateDTO dto);
        Task<QuestionChapterDTO> DeleteSoftAsync(Guid id);
        Task<QuestionChapterDTO> DeleteHardAsync(Guid id);
    }
}