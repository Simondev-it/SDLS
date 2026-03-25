using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IQuestionChapterRepository
    {
        Task<IEnumerable<QuestionChapter>> GetAllAsync(
            Guid? id = null,
            Guid? drivingLicenseId = null,
            string? name = null,
            string? description = null,
            int? status = null,
            string? role = null);

        Task<QuestionChapter?> GetByIdAsync(Guid id, string? role = null);
        Task<QuestionChapter?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(QuestionChapter chapter);
        Task UpdateAsync(QuestionChapter chapter);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}