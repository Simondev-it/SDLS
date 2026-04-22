using SDLS.Model.DTOs;
using SDLS.Model.DTOs.QuestionCategory;
using Microsoft.AspNetCore.Http;

namespace SDLS.Services.Interfaces
{
    public interface IQuestionCategoryService
    {
        Task<List<QuestionCategoryDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null);

        Task<PagedResult<QuestionCategoryDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<(byte[] Content, string FileName, string ContentType)> GenerateImportTemplateAsync();
        Task<List<QuestionCategoryDTO>> ImportAsync(IFormFile file);
        Task<(byte[] Content, string FileName, string ContentType)> ExportToExcelAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null);

        Task<QuestionCategoryDTO> GetByIdAsync(Guid id);
        Task<QuestionCategoryDTO> CreateAsync(QuestionCategoryCreateDTO dto);
        Task<QuestionCategoryDTO> UpdateAsync(Guid id, QuestionCategoryUpdateDTO dto);
        Task<QuestionCategoryDTO> DeleteSoftAsync(Guid id);
        Task<QuestionCategoryDTO> DeleteHardAsync(Guid id);
    }
}