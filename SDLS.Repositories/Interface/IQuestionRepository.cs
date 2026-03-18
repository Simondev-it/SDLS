using Microsoft.EntityFrameworkCore.Storage;
using SDLS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Repositories.Interface
{
    public interface IQuestionRepository
    {
        Task<Question> GetByIdAsync(Guid id);
        Task<IEnumerable<Question>> GetAllAsync();
        Task AddAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteAsync(Guid id);
        Task<Question?> GetChildQuestionAsync(Guid parentId);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
