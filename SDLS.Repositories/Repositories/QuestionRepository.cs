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
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {

        public async Task<Question> GetByIdAsync(Guid id)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id && q.Status == 1);
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.Status == 1)
                .ToListAsync();
        }

        public async Task AddAsync(Question question)
        {
            this.Create(question);
        }

        public async Task UpdateAsync(Question question)
        {
            this.Update(question);
        }

        public async Task DeleteAsync(Guid id)
        {
            var question = this.GetById(id);
            if (question != null)
            {
                question.Status = 0; 
                this.Update(question);
            }
        }
    }
}
