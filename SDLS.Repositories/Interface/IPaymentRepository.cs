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
        Task<Payment> GetFirstOrDefaultAsync(Expression<Func<Payment, bool>> predicate);

        Task<Payment?> GetByBookingIdAsync(int bookingId);

        Task<Payment> GetByOrderIdAsync(int orderId);

        Task<Payment> GetAsync(Expression<Func<Payment, bool>> predicate);

        Task AddAsync(Payment payment);

        void RemoveRange(IEnumerable<Payment> payments);

        Task<IEnumerable<Payment>> GetAllAsync(Expression<Func<Payment, bool>> predicate);

        Task UpdateAsync1(Payment payment);
    }
}
