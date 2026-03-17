using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.Models
{
    public class LessonImage
    {
        public Guid Id { get; set; }
        public Guid QuestionLessonId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? Status { get; set; }
        public virtual QuestionLesson QuestionLesson { get; set; } = null!;
    }
}
