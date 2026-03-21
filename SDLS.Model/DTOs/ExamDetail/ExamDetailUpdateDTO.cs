using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.ExamDetail
{
    public class ExamDetailUpdateDTO
    {
        public Guid? Id { get; set; }

        [NotEmptyGuid]
        public Guid ExamSessionId { get; set; }

        [NotEmptyGuid]
        public Guid AnswerId { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}