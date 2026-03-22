using SDLS.Model.Validations;

namespace SDLS.Model.DTOs.SavedQuestion
{
    public class SavedQuestionCreateDTO
    {
        [NotEmptyGuid]
        public Guid UserId { get; set; }

        [NotEmptyGuid]
        public Guid QuestionId { get; set; }
    }
}