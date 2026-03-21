using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.QuestionTag
{
    public class QuestionTagUpdateDTO
    {
        public Guid? Id { get; set; }

        [NotEmptyGuid]
        public Guid QuestionId { get; set; }

        [NotEmptyGuid]
        public Guid TagId { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}
