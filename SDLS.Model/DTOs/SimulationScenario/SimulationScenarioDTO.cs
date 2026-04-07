using SDLS.Model.DTOs.SimulationCategory;
using SDLS.Model.DTOs.SimulationChapter;
using SDLS.Model.DTOs.SimulationDifficultyLevel;

namespace SDLS.Model.DTOs.SimulationScenario
{
    public class SimulationScenarioDTO
    {
        public Guid Id { get; set; }
        public Guid SimulationChapterId { get; set; }
        public Guid SimulationCategoryId { get; set; }
        public Guid SimulationDifficultyLevelId { get; set; }
        public int? Index { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Video { get; set; }
        public double TotalTime { get; set; }
        public double StartPoint { get; set; }
        public double EndPoint { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        //public SimulationDifficultyLevelBriefDTO? SimulationDifficultyLevel { get; set; }
        //public SimulationCategoryBriefDTO? SimulationCategory { get; set; }
        //public SimulationChapterBriefDTO? SimulationChapter { get; set; }
        public SimulationDifficultyLevelDTO? SimulationDifficultyLevel { get; set; }
        public SimulationCategoryDTO? SimulationCategory { get; set; }
        public SimulationChapterDTO? SimulationChapter { get; set; }
    }
}