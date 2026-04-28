using Microsoft.AspNetCore.Http;

namespace SDLS.Model.DTOs.Question
{
    public class ImportQuestionRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
