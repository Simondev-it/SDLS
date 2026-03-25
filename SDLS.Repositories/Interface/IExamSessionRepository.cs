using SDLS.Model.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SDLS.Repositories.Interface
{
    public interface IExamSessionRepository
    {
        Task<IEnumerable<ExamSession>> GetAllAsync();
        Task<ExamSession?> GetByIdAsync(Guid id);
        Task<ExamSession?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(ExamSession examSession);
        Task UpdateAsync(ExamSession examSession);

        // Giữ hành vi cũ (soft delete)
        Task DeleteAsync(Guid id);

        // Mới
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}