using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.LearningProgress
{
    public class LearningProgressCreateDTO
    {
        public Guid QuestionId { get; set; }
        public Guid UserId { get; set; }
        public int? Status { get; set; } = 1;
    }
}
