using SDLS.Model.DTOs.Answer;
using SDLS.Model.DTOs.QuestionTag;
using System;
using System.Collections.Generic;

namespace SDLS.Model.DTOs.Question
{
    public class QuestionDTO
    {
        public Guid? Id { get; set; }
        public Guid QuestionLessonId { get; set; }
        public Guid QuestionTopicId { get; set; }
        public Guid QuestionCategoryId { get; set; }
        public Guid? ParentId { get; set; }
        public int Position { get; set; }
        public string Content { get; set; } = null!;
        public string? Image { get; set; }
        public string? Explanation { get; set; }
        public string? Type { get; set; }
        public int? Status { get; set; }

        public List<AnswerDTO> Answers { get; set; } = new();
        public List<QuestionTagDTO> QuestionTags { get; set; } = new();
    }
}
