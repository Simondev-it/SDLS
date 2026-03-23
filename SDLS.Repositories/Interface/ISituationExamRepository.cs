using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface ISituationExamRepository
    {
        Task<List<SituationExam>> GetAllAsync(
            Guid? id = null,
            string? title = null,
            string? description = null,
            bool? isRandom = null);

        Task<SituationExam?> GetByIdAsync(Guid id);
        Task<SituationExam?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(SituationExam entity);
        Task UpdateAsync(SituationExam entity);
        Task DeleteAsync(Guid id);
    }
}