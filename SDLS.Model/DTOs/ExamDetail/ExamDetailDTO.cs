using SDLS.Model.DTOs.Answer;
using System;

namespace SDLS.Model.DTOs.ExamDetail
{
    public class ExamDetailDTO
    {
        public Guid Id { get; set; }
        public Guid AnswerId { get; set; }
        public Guid ExamSessionId { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public AnswerDTO? Answer { get; set; }
    }
}