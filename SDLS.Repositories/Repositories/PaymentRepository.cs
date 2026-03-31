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
    //public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    //{
    //    private readonly AppDbContext _context;

    //    public PaymentRepository(AppDbContext context)
    //    {
    //        _context = context;
    //    }

    //    // ================= GET =================

    //    public async Task<Payment?> GetFirstOrDefaultAsync(Expression<Func<Payment, bool>> predicate)
    //    {
    //        return await _context.Payments.FirstOrDefaultAsync(predicate);
    //    }

    //    public async Task<Payment?> GetAsync(Expression<Func<Payment, bool>> predicate)
    //    {
    //        return await _context.Payments.FirstOrDefaultAsync(predicate);
    //    }

    //    public async Task<IEnumerable<Payment>> GetAllAsync(Expression<Func<Payment, bool>> predicate)
    //    {
    //        return await _context.Payments.Where(predicate).ToListAsync();
    //    }

    //    //public async Task<Payment?> GetByBookingIdAsync(int bookingId)
    //    //{
    //    //    return await _context.Payments
    //    //        .Where(p => p.BookingId == bookingId && p.Status == 0) // chưa thanh toán
    //    //        .OrderByDescending(p => p.Id)
    //    //        .FirstOrDefaultAsync();
    //    //}

    //    //public async Task<Payment?> GetByOrderIdAsync(int orderId)
    //    //{
    //    //    // BookingId = OrderId theo design của bạn
    //    //    return await _context.Payments
    //    //        .OrderByDescending(p => p.Id)
    //    //        .FirstOrDefaultAsync(p => p.BookingId == orderId);
    //    //}

    //    public async Task<bool> ExistsAsync(Expression<Func<Payment, bool>> predicate)
    //    {
    //        return await _context.Payments.AnyAsync(predicate);
    //    }

    //    // ================= CREATE =================

    //    public async Task AddAsync(Payment payment)
    //    {
    //        await _context.Payments.AddAsync(payment);
    //        await _context.SaveChangesAsync();
    //    }

    //    // ================= UPDATE =================

    //    /// <summary>
    //    /// Update các field cần thiết (dùng cho callback PayOS)
    //    /// </summary>
    //    public async Task UpdateAsync(Payment payment)
    //    {
    //        _context.Payments.Update(payment);
    //        await _context.SaveChangesAsync();
    //    }

    //    /// <summary>
    //    /// Update trạng thái theo OrderId (quan trọng cho PayOS callback)
    //    /// </summary>
    //    //public async Task UpdateStatusByOrderIdAsync(int orderId, int status, string response)
    //    //{
    //    //    var payment = await _context.Payments
    //    //        .OrderByDescending(p => p.Id)
    //    //        .FirstOrDefaultAsync(p => p.BookingId == orderId);

    //    //    if (payment == null)
    //    //        return;

    //    //    payment.Status = status;
    //    //    payment.Response = response;
    //    //    payment.Date = DateTime.UtcNow;

    //    //    await _context.SaveChangesAsync();
    //    //}

    //    // ================= DELETE =================

    //    public void RemoveRange(IEnumerable<Payment> payments)
    //    {
    //        _context.Payments.RemoveRange(payments);
    //    }
    //}
}
