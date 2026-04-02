using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.Answer
{
    public class AnswerDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public bool? Iscorrect { get; set; }
        public int ? Status { get; set; }
    }
}
