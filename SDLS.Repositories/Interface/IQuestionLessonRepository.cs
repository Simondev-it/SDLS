using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IQuestionLessonRepository
    {
        Task<IEnumerable<QuestionLesson>> GetAllAsync(
            Guid? id = null,
            Guid? questionChapterId = null,
            string? name = null,
            string? description = null,
            string? content = null,
            int? status = 1);
        Task<QuestionLesson?> GetByIdAsync(Guid id);
        Task<QuestionLesson?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(QuestionLesson lesson);
        Task UpdateAsync(QuestionLesson lesson);
        Task DeleteAsync(Guid id);

        Task<List<LessonImage>> GetLessonImagesByLessonIdsAsync(List<Guid> lessonIds);
        Task<List<LessonImage>> GetLessonImagesByLessonIdForUpdateAsync(Guid lessonId);
        void RemoveLessonImages(IEnumerable<LessonImage> images);
        void AddLessonImages(IEnumerable<LessonImage> images);
        Task SoftDeleteLessonImagesAsync(Guid lessonId, DateTime now);
    }
}