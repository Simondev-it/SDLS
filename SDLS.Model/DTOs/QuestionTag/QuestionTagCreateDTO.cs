using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.QuestionTag
{
    public class QuestionTagCreateDTO
    {
        public Guid QuestionId { get; set; }

        public Guid TagId { get; set; }
    }
}
