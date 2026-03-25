using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class CommentVoteRepository : GenericRepository<CommentVote>, ICommentVoteRepository
    {
        public async Task<List<CommentVote>> GetAllAsync()
        {
            return await _context.CommentVotes
                .Include(x => x.ForumComment)
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CommentVote?> GetByIdAsync(Guid id)
        {
            return await _context.CommentVotes
                .Include(x => x.ForumComment)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<List<CommentVote>> GetByUserAndForumCommentAsync(Guid? userId, Guid? forumCommentId)
        {
            var query = _context.CommentVotes
                .Include(x => x.ForumComment)
                .Where(x => x.Status == 1);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (forumCommentId.HasValue)
                query = query.Where(x => x.ForumCommentId == forumCommentId.Value);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(CommentVote entity)
        {
            await _context.CommentVotes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CommentVote entity)
        {
            _context.CommentVotes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.CommentVotes
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);

            if (existing == null)
                return;

            existing.Status = 0;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.CommentVotes
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return;

            _context.CommentVotes.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}