using SDLS.Model.DTOs;
using SDLS.Model.DTOs.QuestionCategory;

namespace SDLS.Services.Interfaces
{
    public interface IQuestionCategoryService
    {
        Task<List<QuestionCategoryDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null);

        Task<PagedResult<QuestionCategoryDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int page = 1,
            int pageSize = 20);

        Task<QuestionCategoryDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(QuestionCategoryCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, QuestionCategoryUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}