using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.Report
{
    public class ReportResolveActionDTO
    {
        [Required(ErrorMessage = "Tr??ng này là b?t bu?c.")]
        [StringLength(255, ErrorMessage = "V??t quá ?? dài t?i ?a 255 ký t?.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Tr??ng này là b?t bu?c.")]
        [StringLength(1000, ErrorMessage = "V??t quá ?? dài t?i ?a 1000 ký t?.")]
        public string Content { get; set; } = null!;
    }
}
