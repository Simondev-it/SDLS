using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.LearningProgress
{
    public class LearningProgressUpdateDTO
    {
        [NotEmptyGuid]
        public Guid QuestionId { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}
