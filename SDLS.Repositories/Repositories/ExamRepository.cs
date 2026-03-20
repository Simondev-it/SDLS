using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDLS.Repositories.Repositories
{
    public class ExamRepository : GenericRepository<Exam>, IExamRepository
    {
        public async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await _context.Exams
                .Where(e => e.Status == 1 && e.IsRandom == false)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Exam?> GetByIdAsync(Guid id)
        {
            return await _context.Exams
                .Include(e => e.ExamQuestions.Where(eq => eq.Status == 1))
                .Where(e => e.Status == 1)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Exam?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.Exams
                .Include(e => e.ExamQuestions)
                .FirstOrDefaultAsync(e => e.Id == id && e.Status == 1);
        }

        public async Task AddAsync(Exam exam)
        {
            await _context.Exams.AddAsync(exam);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Exam exam)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == id && e.Status == 1);
            if (exam == null)
                return;

            exam.Status = 0;
            exam.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }
    }
}
