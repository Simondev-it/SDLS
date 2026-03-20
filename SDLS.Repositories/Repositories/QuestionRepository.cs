using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
                .Include(q => q.Answers.Where(a => a.Status == 1))
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id && q.Status == 1);
        }

        public async Task<Question> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id && q.Status == 1);
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _context.Questions
                .Include(q => q.Answers.Where(a => a.Status == 1))
                .Include(q => q.InverseParent)
                .Where(q => q.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Question question)
        {
            this.CreateAsync(question);
        }
        //_context.Update(question); 
        public async Task UpdateAsync(Question question)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAnswersByQuestionIdAsync(Guid questionId)
        {
            var answers = await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();

            _context.Answers.RemoveRange(answers);
            await _context.SaveChangesAsync();
        }

        public async Task AddAnswerAsync(Answer answer)
        {
            _context.Answers.AddAsync(answer);
            await SaveAsync();  // hoặc Prepare + Save riêng tùy thiết kế
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

        public async Task<Question?> GetChildQuestionAsync(Guid parentId)
        {
            return this.GetById(parentId)?.InverseParent.FirstOrDefault(q => q.Status == 1);
        }

        public async Task<List<Question>> GetAllByLessonAsync(Guid lessonId)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .Include(q => q.InverseParent)   // cần để traverse next
                .Where(q => q.QuestionLessonId == lessonId && q.Status == 1)
                .AsNoTracking()                  // tăng tốc
                .ToListAsync();
        }

        public async Task<Question?> GetByIdWithLinksAsync(Guid id)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .Include(q => q.InverseParent)
                .FirstOrDefaultAsync(q => q.Id == id && q.Status == 1);
        }

        public async Task UpdateParentIdAsync(Guid questionId, Guid? newParentId)
        {
            await _context.Questions
                .Where(q => q.Id == questionId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(q => q.ParentId, newParentId)
                    .SetProperty(q => q.UpdateAt, DateTime.UtcNow.ToLocalTime()));
        }

        public async Task<List<Question>> GetLessonQuestionsForReorderAsync(Guid lessonId)
        {
            return await _context.Questions
                .Where(q => q.QuestionLessonId == lessonId && q.Status == 1)
                .Select(q => new Question { Id = q.Id, ParentId = q.ParentId }) // chỉ lấy 2 field
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
