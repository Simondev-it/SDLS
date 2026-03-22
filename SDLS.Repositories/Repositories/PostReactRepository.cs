using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class PostReactRepository : GenericRepository<PostReact>, IPostReactRepository
    {
        public async Task<List<PostReact>> GetAllAsync()
        {
            return await _context.PostReacts
                .Where(x => x.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PostReact?> GetByIdAsync(Guid id)
        {
            return await _context.PostReacts
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
        }

        public async Task<List<PostReact>> GetByUserAndForumPostAsync(Guid? userId, Guid? forumPostId)
        {
            var query = _context.PostReacts.Where(x => x.Status == 1);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (forumPostId.HasValue)
                query = query.Where(x => x.ForumPostId == forumPostId.Value);

            return await query.ToListAsync();
        }

        public async Task AddAsync(PostReact entity)
        {
            await _context.PostReacts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PostReact entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.PostReacts.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}