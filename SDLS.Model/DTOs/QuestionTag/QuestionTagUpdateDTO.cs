using System;

namespace SDLS.Model.DTOs.QuestionTag
{
    public class QuestionTagUpdateDTO
    {
        public Guid? Id { get; set; }
        public Guid QuestionId { get; set; }
        public Guid TagId { get; set; }
        public int? Status { get; set; }
    }
}
