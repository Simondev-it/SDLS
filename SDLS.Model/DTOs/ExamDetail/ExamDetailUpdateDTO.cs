using System;

namespace SDLS.Model.DTOs.ExamDetail
{
    public class ExamDetailUpdateDTO
    {
        public Guid? Id { get; set; }
        public Guid ExamSessionId { get; set; }
        public Guid AnswerId { get; set; }
        public int? Status { get; set; }
    }
}