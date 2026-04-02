using SDLS.Model.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SDLS.Repositories.Interface
{
    public interface IExamSessionRepository
    {
        Task<IEnumerable<ExamSession>> GetAllAsync(
            Guid? examId = null,
            Guid? userId = null,
            int? status = null,
            string? role = null);

        Task<ExamSession?> GetByIdAsync(Guid id, string? role = null);
        Task<ExamSession?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(ExamSession examSession);
        Task UpdateAsync(ExamSession examSession);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}