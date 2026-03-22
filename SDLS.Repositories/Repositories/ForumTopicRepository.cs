using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class ForumTopicRepository : GenericRepository<ForumTopic>, IForumTopicRepository
    {
        public async Task<List<ForumTopic>> GetAllAsync()
        {
            return await _context.ForumTopics
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ForumTopic?> GetByIdAsync(Guid id)
        {
            return await _context.ForumTopics
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<ForumTopic?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.ForumTopics
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task AddAsync(ForumTopic entity)
        {
            await _context.ForumTopics.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ForumTopic entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _context.ForumTopics.FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
            if (existing == null) return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();

            await _context.SaveChangesAsync();
        }
    }
}