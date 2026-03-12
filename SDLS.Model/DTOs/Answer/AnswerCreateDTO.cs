using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.Answer
{
    public class AnswerCreateDTO
    {
        public string Content { get; set; } = null!;
        public bool Iscorrect { get; set; }
    }
}
