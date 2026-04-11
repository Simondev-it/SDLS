using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SavedQuestion;

namespace SDLS.Services.Interfaces
{
    public interface ISavedQuestionService
    {
        Task<List<SavedQuestionDTO>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionId = null,
            int? status = null);

        Task<PagedResult<SavedQuestionDTO>> GetPagedAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionId = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<SavedQuestionDTO?> GetByIdAsync(Guid id);
        Task<SavedQuestionDTO> CreateAsync(SavedQuestionCreateDTO dto);
        Task<SavedQuestionDTO> UpdateAsync(Guid id, SavedQuestionUpdateDTO dto);
        Task<SavedQuestionDTO> DeleteSoftAsync(Guid id);
        Task<SavedQuestionDTO> DeleteHardAsync(Guid id);
    }
}