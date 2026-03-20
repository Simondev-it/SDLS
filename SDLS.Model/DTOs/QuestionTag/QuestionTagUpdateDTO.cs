using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.QuestionTag
{
    public class QuestionTagUpdateDTO
    {
        public Guid Id { get; set; }

        public Guid QuestionId { get; set; }

        public Guid TagId { get; set; }
        
        public int? Status { get; set; }
    }
}
