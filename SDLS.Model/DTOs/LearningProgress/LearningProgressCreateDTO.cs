using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDLS.Model.Validations;

namespace SDLS.Model.DTOs.LearningProgress
{
    public class LearningProgressCreateDTO
    {
        [NotEmptyGuid]
        public Guid QuestionId { get; set; }

        [NotEmptyGuid]
        public Guid UserId { get; set; }
    }
}
