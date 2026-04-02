using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.ForumPost
{
    public class ForumPostCreateDTO
    {
        [NotEmptyGuid]
        public Guid ForumTopicId { get; set; }

        [StringLength(255, ErrorMessage = "Vuot qua do dai toi da 255 ky tu.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Truong nay la bat buoc.")]
        [StringLength(255, ErrorMessage = "Vuot qua do dai toi da 255 ky tu.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Truong nay la bat buoc.")]
        [StringLength(255, ErrorMessage = "Vuot qua do dai toi da 255 ky tu.")]
        public string Content { get; set; } = null!;
    }
}
