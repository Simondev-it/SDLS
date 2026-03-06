using SDLS.Model.DTOs.Answer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.Question
{
    public class QuestionCreateDTO
    {
        public Guid? Questioncategoryid { get; set; }
        public Guid? Questiondifficultylevelid { get; set; }
        public string Content { get; set; } = null!;
        public string? Image { get; set; }
        public string? Explanation { get; set; }
        public bool? Issingleanswer { get; set; }
        public int? Status { get; set; }
        public List<AnswerCreateDTO> Answers { get; set; } = new List<AnswerCreateDTO>();
    }
}
