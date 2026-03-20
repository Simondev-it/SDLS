using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.Answer.ExamQuestion
{
    public class ExamQuestionDTO
    {
        public Guid Id { get; set; }
        public Guid ExamId { get; set; }
        public Guid QuestionId { get; set; }
        public int Order { get; set; }
        public int? Status { get; set; }
    }
}
