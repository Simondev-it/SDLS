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
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CommentVote?> GetByIdAsync(Guid id)
        {
            return await _context.CommentVotes
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<List<CommentVote>> GetByUserAndForumCommentAsync(Guid? userId, Guid? forumCommentId)
        {
            var query = _context.CommentVotes.Where(x => x.Status == 1);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (forumCommentId.HasValue)
                query = query.Where(x => x.ForumCommentId == forumCommentId.Value);

            return await query.ToListAsync();
        }

        public async Task AddAsync(CommentVote entity)
        {
            await _context.CommentVotes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CommentVote entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.CommentVotes.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}