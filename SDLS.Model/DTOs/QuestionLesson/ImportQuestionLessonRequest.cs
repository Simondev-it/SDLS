using Microsoft.AspNetCore.Http;

namespace SDLS.Model.DTOs.QuestionLesson
{
    public class ImportQuestionLessonRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
