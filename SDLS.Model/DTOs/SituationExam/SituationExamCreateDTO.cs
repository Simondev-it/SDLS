using SDLS.Model.DTOs.SituationExam;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.SituationExam
{
    public class SituationExamCreateDTO
    {
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        
        public string Title { get; set; } = null!;

        
        public string? Description { get; set; }

        [Range(0, 100, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? PassScore { get; set; }

        public bool IsRandom { get; set; }

        public List<SimulationExamCreateDTO> SimulationExams { get; set; } = new();
    }
}