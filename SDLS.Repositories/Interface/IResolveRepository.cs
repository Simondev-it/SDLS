using Microsoft.EntityFrameworkCore.Storage;
using SDLS.Model.Models;

namespace SDLS.Repositories.Interface
{
    public interface IResolveRepository
    {
        Task<List<Resolve>> GetAllAsync(
            Guid? id = null,
            Guid? reportId = null,
            Guid? userId = null,
            string? title = null,
            string? content = null,
            int? status = 1);

        Task<Resolve?> GetByIdAsync(Guid id);
        Task<Resolve?> GetByIdForUpdateAsync(Guid id);
        Task AddAsync(Resolve entity);
        Task UpdateAsync(Resolve entity);
        Task DeleteSoftAsync(Guid id);
        Task DeleteHardAsync(Guid id);

        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}