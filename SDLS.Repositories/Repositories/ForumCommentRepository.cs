using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class ForumCommentRepository : GenericRepository<ForumComment>, IForumCommentRepository
    {
        public async Task<IEnumerable<ForumComment>> GetAllAsync()
        {
            return await _context.ForumComments
                .Include(x => x.User)
                .Include(x => x.ForumPost)
                .Include(x => x.CommentVotes.Where(v => v.Status == 1))
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ForumComment?> GetByIdAsync(Guid id)
        {
            return await _context.ForumComments
                .Include(x => x.User)
                .Include(x => x.ForumPost)
                .Include(x => x.CommentVotes.Where(v => v.Status == 1))
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ForumComment?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.ForumComments
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task AddAsync(ForumComment entity)
        {
            await _context.ForumComments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ForumComment entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.ForumComments
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.ForumComments
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            _context.ForumComments.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
