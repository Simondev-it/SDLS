using SDLS.Model.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SDLS.Repositories.Interface
{
    public interface IExamRepository
    {
        Task<IEnumerable<Exam>> GetAllAsync(
            Guid? userId = null,
            int? status = null,
            string? role = null);

        Task<Exam?> GetByIdAsync(Guid id, string? role = null);
        Task<Exam?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(Exam exam);
        Task UpdateAsync(Exam exam);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);
    }
}
