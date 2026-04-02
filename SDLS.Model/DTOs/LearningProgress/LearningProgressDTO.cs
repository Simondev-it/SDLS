using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDLS.Model.DTOs.Question;

namespace SDLS.Model.DTOs.LearningProgress
{
    public class LearningProgressDTO
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public Guid UserId { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? Status { get; set; }

        public QuestionDTO? Question { get; set; }
    }
}
