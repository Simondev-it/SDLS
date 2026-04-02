using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ILessonProgressRepository
    {
        Task<List<LessonProgress>> GetAllAsync(
            Guid? id = null,
            Guid? userId = null,
            Guid? questionLessonId = null,
            int? status = null,
            string? role = null);

        Task<LessonProgress?> GetByIdAsync(Guid id, string? role = null);
        Task<LessonProgress?> GetByIdForUpdateAsync(Guid id);
        Task<List<LessonProgress>> GetByUserAndQuestionLessonAsync(
            Guid? userId,
            Guid? questionLessonId,
            int? status = null,
            string? role = null);

        Task AddAsync(LessonProgress entity);
        Task UpdateAsync(LessonProgress entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}