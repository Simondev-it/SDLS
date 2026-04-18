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
    public class LessonImageRepository : GenericRepository<LessonImage>, ILessonImageRepository
    {
        public async Task AddAsync(LessonImage image)
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

        public async Task<LessonImage> GetByIdAsync(Guid id)
        {
            return await _context.LessonImages.Include(l => l.QuestionLesson).FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<LessonImage>> GetByLessonIdAsync(Guid id)
        {
            return await _context.LessonImages.Include(l => l.QuestionLesson).Where(l => l.QuestionLessonId == id).ToListAsync();
        }

        public async Task<IEnumerable<LessonImage>> GetAllAsync()
        {
            return await _context.LessonImages.Include(l => l.QuestionLesson).ToListAsync();
        }
    }
}
