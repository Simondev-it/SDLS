using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ILessonProgressRepository
    {
        Task<List<LessonProgress>> GetAllAsync();
        Task<LessonProgress?> GetByIdAsync(Guid id);
        Task<LessonProgress?> GetByIdForUpdateAsync(Guid id);
        Task<List<LessonProgress>> GetByUserAndQuestionLessonAsync(Guid? userId, Guid? questionLessonId);
        Task AddAsync(LessonProgress entity);
        Task UpdateAsync(LessonProgress entity);
        Task DeleteAsync(Guid id);
    }
}