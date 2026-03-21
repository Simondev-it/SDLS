using SDLS.Model.DTOs.LearningProgress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Services.Interfaces
{
    public interface ILearningProgressService
    {
        Task<IEnumerable<LearningProgressDTO>> GetAllAsync();
        Task<LearningProgressDTO?> GetByIdAsync(Guid id);
        Task<LearningProgressDTO> CreateAsync(LearningProgressCreateDTO dto);
        Task<LearningProgressDTO?> UpdateAsync(Guid id, LearningProgressUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
        Task<List<LearningProgressDTO>> GetByUserAndQuestionAsync(Guid? userId, Guid? questionId);
    }
}
