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
            int? status = 1,
            int page = 1,
            int pageSize = 20);

        Task<QuestionChapterDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(QuestionChapterCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, QuestionChapterUpdateDTO dto);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}