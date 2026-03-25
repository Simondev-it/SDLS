using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class QuestionTopicRepository : GenericRepository<QuestionTopic>, IQuestionTopicRepository
    {
        public async Task<List<QuestionTopic>> GetAllAsync()
        {
            return await _context.QuestionTopics
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<QuestionTopic?> GetByIdAsync(Guid id)
        {
            return await _context.QuestionTopics
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<QuestionTopic?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.QuestionTopics
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task AddAsync(QuestionTopic entity)
        {
            await _context.QuestionTopics.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(QuestionTopic entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.QuestionTopics
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.QuestionTopics
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            _context.QuestionTopics.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}