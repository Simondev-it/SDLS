using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDLS.Model.Validations;

namespace SDLS.Model.DTOs.QuestionTag
{
    public class QuestionTagCreateDTO
    {
        [NotEmptyGuid]
        public Guid TagId { get; set; }
    }
}
