using SDLS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Repositories.Interface
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetAsync(Expression<Func<Payment, bool>> predicate);

        Task<IEnumerable<Payment>> GetAllAsync(Expression<Func<Payment, bool>> predicate);

        Task<Payment?> GetByOrderCodeAsync(long orderCode);

        Task<Payment?> GetPendingByUserIdAsync(Guid userId);

        Task AddAsync(Payment payment);

        Task UpdateAsync(Payment payment);

        Task UpdateStatusByOrderCodeAsync(long orderCode, int status, string response);
    }
}
