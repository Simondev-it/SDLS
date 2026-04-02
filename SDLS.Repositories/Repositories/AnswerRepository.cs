using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class AnswerRepository : GenericRepository<Answer>, IAnswerRepository
    {
        public async Task AddAsync(Answer answer)
        {
            this.Create(answer);
        }

        public async Task UpdateAsync(Answer answer)
        {
            await _context.SaveChangesAsync();
        }

        

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.Answers
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.Answers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            var examDetails = await _context.ExamDetails
                .Where(x => x.AnswerId == id)
                .ToListAsync();

            if (examDetails.Count > 0)
                _context.ExamDetails.RemoveRange(examDetails);

            _context.Answers.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
