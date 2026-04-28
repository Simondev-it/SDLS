using SDLS.Model.DTOs;
using SDLS.Model.DTOs.Question;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SDLS.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<PagedResult<QuestionDTO>> GetAllAsync(
            Guid? lessonId = null,
            Guid? topicId = null,
            Guid? QuestionCategoryId = null,
            List<Guid>? tagIds = null,
            string? searchContent = null,
            int? status = null,
            int page = 1,
            int pageSize = 20,
            string? sortBy = null);

        Task<QuestionDTO> GetByIdAsync(Guid id);
        Task<QuestionDTO> GetByIdForAdminAsync(Guid id);
        Task<QuestionDTO> CreateAsync(QuestionCreateDTO dto);
        Task<List<QuestionDTO>> CreateManyAsync(List<QuestionCreateDTO> dtos);
        Task<(byte[] Content, string FileName, string ContentType)> GenerateImportTemplateAsync();
        Task<(byte[] Content, string FileName, string ContentType)> ExportLicenseChapterLessonAsync();
        Task<List<QuestionDTO>> ImportAsync(IFormFile file);
        Task<QuestionDTO> UpdateAsync(Guid id, QuestionUpdateDTO dto);
        Task<QuestionDTO> DeleteSoftAsync(Guid id);
        Task<QuestionDTO> DeleteHardAsync(Guid id);
    }
}
