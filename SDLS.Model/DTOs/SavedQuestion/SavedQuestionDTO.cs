using SDLS.Model.DTOs.Question;

namespace SDLS.Model.DTOs.SavedQuestion
{
    public class SavedQuestionDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid QuestionId { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public QuestionDTO? Question { get; set; }
    }
}