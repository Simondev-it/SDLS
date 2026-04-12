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
            int? status = null,
            string? role = null);

        Task<QuestionLesson?> GetByIdAsync(Guid id, string? role = null);
        Task<QuestionLesson?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(QuestionLesson lesson);
        Task UpdateAsync(QuestionLesson lesson);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);

        Task<List<LessonImage>> GetLessonImagesByLessonIdsAsync(List<Guid> lessonIds, string? role = null);
        Task<List<LessonImage>> GetLessonImagesByLessonIdForUpdateAsync(Guid lessonId);
        void RemoveLessonImages(IEnumerable<LessonImage> images);
        void AddLessonImages(IEnumerable<LessonImage> images);
        Task SoftDeleteLessonImagesAsync(Guid lessonId, DateTime now);
        Task RestoreLessonImagesAsync(Guid lessonId, DateTime now);
    }
}