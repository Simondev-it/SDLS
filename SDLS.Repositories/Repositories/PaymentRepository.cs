using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Repositories.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext
            
             context)
        {
            _context = context;
        }

        public async Task<Payment?> GetAsync(Expression<Func<Payment, bool>> predicate)
        {
            return await _context.Payments.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Payment>> GetAllAsync(Expression<Func<Payment, bool>> predicate)
        {
            return await _context.Payments.Where(predicate).ToListAsync();
        }

        public async Task<Payment?> GetByOrderCodeAsync(long orderCode)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderCode == orderCode);
        }

        public async Task<Payment?> GetPendingByUserIdAsync(Guid userId)
        {
            return await _context.Payments
                .Where(p => p.UserId == userId && p.Status == 0)
                .OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Payment payment)
        {
            payment.Id = Guid.NewGuid();
            payment.CreateAt = DateTime.UtcNow;
            payment.Status = 0;

            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Payment payment)
        {
            payment.UpdateAt = DateTime.UtcNow;
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusByOrderCodeAsync(long orderCode, int status, string response)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderCode == orderCode);

            if (payment == null) return;

            payment.Status = status;
            payment.Response = response;
            payment.UpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .OrderByDescending(p => p.CreateAt)
                .ToListAsync();
        }
    }
}
