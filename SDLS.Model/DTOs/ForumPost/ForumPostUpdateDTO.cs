using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.ForumPost
{
    public class ForumPostUpdateDTO
    {
        public Guid? ForumTopicId { get; set; }
        public Guid? UserId { get; set; }

        [StringLength(255, ErrorMessage = "Vuot qua do dai toi da 255 ky tu.")]
        public string? Name { get; set; }

        [StringLength(255, ErrorMessage = "Vuot qua do dai toi da 255 ky tu.")]
        public string? Title { get; set; }

        [StringLength(255, ErrorMessage = "Vuot qua do dai toi da 255 ky tu.")]
        public string? Content { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Gia tri khong hop le.")]
        public int? ViewCount { get; set; }

        [Range(0, 1, ErrorMessage = "Gia tri khong hop le.")]
        public int? Status { get; set; }
    }
}
