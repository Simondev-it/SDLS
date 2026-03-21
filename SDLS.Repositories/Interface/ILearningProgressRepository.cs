using SDLS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Repositories.Interface
{
    public interface ILearningProgressRepository
    {
        Task<LearningProgress?> GetByIdAsync(Guid id);
        Task<List<LearningProgress>> GetAllAsync();
        Task AddAsync(LearningProgress entity);
        Task UpdateAsync(LearningProgress entity);
        Task DeleteAsync(Guid id); // soft delete
        Task<List<LearningProgress>> GetByUserAndQuestionAsync(Guid? userId, Guid? questionId);
    }
}
