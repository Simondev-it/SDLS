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
    public class QuestionRepository : IQuestionRepository, GenericRepository<Question>
    {
        private readonly SdlsDbContext _context;

        public QuestionRepository(SdlsDbContext context)
        {
            _context = context;
        }

        public async Task<Question> GetByIdAsync(Guid id)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .Include(q => q.Questioncategory)
                .Include(q => q.Questiondifficultylevel)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .Include(q => q.Questioncategory)
                .Include(q => q.Questiondifficultylevel)
                .ToListAsync();
        }

        public async Task AddAsync(Question question)
        {
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var question = await GetByIdAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }
        }
    }
}
