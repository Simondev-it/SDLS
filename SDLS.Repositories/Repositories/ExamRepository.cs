using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Repositories.Repositories
{
    public class ExamRepository : GenericRepository<Exam>, IExamRepository
    {
        public async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await _context.Exams
                .Where(q => q.Status == 1 && q.IsRandom == false)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Exam> GetByIdAsync(Guid id)
        {
            return await _context.Exams
                .Include(q => q.ExamQuestions)
                .Where(q => q.Status == 1)
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task AddAsync(Exam exam)
        {
            this.Create(exam);
        }

        public async Task UpdateAsync(Exam exam)
        {
            this.Update(exam);
        }

        public async Task DeleteAsync(Guid id)
        {
            var exam = this.GetById(id);
            if (exam != null)
            {
                exam.Status = 0;
                this.Update(exam);
            }
        }
    }
}
