using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Resolve
{
    public class ResolveUpdateDTO
    {
        [NotEmptyGuid]
        public Guid ReportId { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        
        public string Content { get; set; } = null!;

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}