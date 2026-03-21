using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Answer.ExamQuestion
{
    public class ExamQuestionUpdateDTO
    {
        public Guid? Id { get; set; }

        [NotEmptyGuid]
        public Guid ExamId { get; set; }

        [NotEmptyGuid]
        public Guid QuestionId { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}