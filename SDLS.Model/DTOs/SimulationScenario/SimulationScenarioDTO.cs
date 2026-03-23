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
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Video { get; set; }
        public int TotalTime { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public int? Status { get; set; }

        //public SimulationDifficultyLevelBriefDTO? SimulationDifficultyLevel { get; set; }
        //public SimulationCategoryBriefDTO? SimulationCategory { get; set; }
        //public SimulationChapterBriefDTO? SimulationChapter { get; set; }
        public SimulationDifficultyLevelDTO? SimulationDifficultyLevel { get; set; }
        public SimulationCategoryDTO? SimulationCategory { get; set; }
        public SimulationChapterDTO? SimulationChapter { get; set; }
    }
}