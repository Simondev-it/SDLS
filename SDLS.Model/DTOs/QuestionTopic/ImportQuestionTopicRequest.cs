using Microsoft.AspNetCore.Http;

namespace SDLS.Model.DTOs.QuestionTopic
{
    public class ImportQuestionTopicRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
