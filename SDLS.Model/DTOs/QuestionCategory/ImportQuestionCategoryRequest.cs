using Microsoft.AspNetCore.Http;

namespace SDLS.Model.DTOs.QuestionCategory
{
    public class ImportQuestionCategoryRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
