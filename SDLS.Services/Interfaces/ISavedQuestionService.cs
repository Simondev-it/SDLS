using SDLS.Model.DTOs;
using SDLS.Model.DTOs.SavedQuestion;

namespace SDLS.Services.Interfaces
{
    public interface ISavedQuestionService
    {
        Task<List<SavedQuestionDTO>> GetAllAsync(Guid? id = null, Guid? userId = null, Guid? questionId = null);
        Task<PagedResult<SavedQuestionDTO>> GetPagedAsync(Guid? id = null, Guid? userId = null, Guid? questionId = null, int page = 1, int pageSize = 20);
        Task<SavedQuestionDTO?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(SavedQuestionCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, SavedQuestionUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}