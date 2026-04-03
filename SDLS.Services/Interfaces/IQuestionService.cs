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
            int pageSize = 20);

        Task<QuestionDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(QuestionCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, QuestionUpdateDTO dto);
        Task<byte[]> DownloadImportTemplateAsync(string format = "xlsx");
        Task<QuestionImportResultDTO> ImportQuestionsAsync(IFormFile file);

        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteSoftAsync(Guid id);
        Task<bool> DeleteHardAsync(Guid id);
    }
}
