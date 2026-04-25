using SDLS.Model.DTOs.QuestionLesson;
using SDLS.Model.DTOs.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.QuestionTag
{
    public class QuestionTagDTO
    {
        public Guid Id { get; set; }

        public Guid QuestionId { get; set; }

        public Guid TagId { get; set; }

        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public TagDTO? Tag { get; set; }
    }
}
