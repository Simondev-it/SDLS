using Microsoft.EntityFrameworkCore;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Interface.ImageInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Repositories.Repositories
{
    public class PostImageRepository : GenericRepository<PostImage>, IPostImageRepository
    {
        public async Task AddAsync(PostImage image)
        {
            this.Create(image);
        }

        public async Task DeleteAsync(Guid id)
        {
            var image = this.GetById(id);
            if (image != null)
            {
                this.Remove(image);
            }
        }

        public async Task<PostImage> GetByIdAsync(Guid id)
        {
            return await _context.PostImages.Include(p => p.ForumPost).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PostImage> GetByPostIdAsync(Guid id)
        {
            return await _context.PostImages.Include(l => l.ForumPost).FirstOrDefaultAsync(l => l.ForumPostId == id);
        }

        public async Task<IEnumerable<PostImage>> GetAllAsync()
        {
            return await _context.PostImages.Include(l => l.ForumPost).ToListAsync();
        }
    };

}
