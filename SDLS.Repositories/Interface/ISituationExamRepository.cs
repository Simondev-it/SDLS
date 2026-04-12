using SDLS.Model.Models;
using System.Threading.Tasks;

namespace SDLS.Repositories.Interface
{
    public interface ISituationExamRepository
    {
        Task<List<SituationExam>> GetAllAsync(
            Guid? id = null,
            string? title = null,
            string? description = null,
            bool? isRandom = null,
            int? status = null,
            string? role = null);

        Task<SituationExam?> GetByIdAsync(Guid id, string? role = null);
        Task<SituationExam?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(SituationExam entity);
        Task UpdateAsync(SituationExam entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);

        Task<int> GetPassScoreAsync(Guid situationExamId);
    }
}