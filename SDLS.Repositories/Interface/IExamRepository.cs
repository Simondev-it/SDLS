using SDLS.Model.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SDLS.Repositories.Interface
{
    public interface IExamRepository
    {
        Task<IEnumerable<Exam>> GetAllAsync();
        Task<Exam?> GetByIdAsync(Guid id);
        Task<Exam?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(Exam exam);
        Task UpdateAsync(Exam exam);
        Task DeleteAsync(Guid id);
    }
}
