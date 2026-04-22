using SDLS.Model.DTOs;
using SDLS.Model.DTOs.QuestionTopic;
using Microsoft.AspNetCore.Http;

namespace SDLS.Services.Interfaces
{
    public interface IQuestionTopicService
    {
        Task<List<QuestionTopicDTO>> GetAllAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null);

        Task<PagedResult<QuestionTopicDTO>> GetPagedAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null,
            int page = 1,
            int pageSize = 20);

        Task<QuestionTopicDTO> GetByIdAsync(Guid id);
        Task<QuestionTopicDTO> CreateAsync(QuestionTopicCreateDTO dto);
        Task<(byte[] Content, string FileName, string ContentType)> GenerateImportTemplateAsync();
        Task<List<QuestionTopicDTO>> ImportAsync(IFormFile file);
        Task<(byte[] Content, string FileName, string ContentType)> ExportToExcelAsync(
            Guid? id = null,
            string? name = null,
            string? description = null,
            int? status = null);
        Task<QuestionTopicDTO> UpdateAsync(Guid id, QuestionTopicUpdateDTO dto);
        Task<QuestionTopicDTO> DeleteSoftAsync(Guid id);
        Task<QuestionTopicDTO> DeleteHardAsync(Guid id);
    }
}