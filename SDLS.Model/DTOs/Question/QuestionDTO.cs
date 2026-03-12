using SDLS.Model.DTOs.Answer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.Question
{
    public class QuestionDTO
    {
        public Guid? Id { get; set; }
        public Guid QuestionLessonId { get; set; }

        public Guid QuestionTopicId { get; set; }

        public Guid QuestionCategoryId { get; set; }

        public Guid? ParentId { get; set; }

        public string Content { get; set; } = null!;

        public string? Image { get; set; }

        public string? Explanation { get; set; }

        public string? Type { get; set; }

        public int? Status { get; set; }

        public List<AnswerDTO> Answers { get; set; } = new();
    }
}
