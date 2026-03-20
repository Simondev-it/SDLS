using Microsoft.EntityFrameworkCore;
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
    public class LearningProgressRepository : GenericRepository<LearningProgress>, ILearningProgressRepository
    {
        public async Task<LearningProgress?> GetByIdAsync(Guid id)
        {
            return await _context.LearningProgresses
                .Include(lp => lp.Question)
                .Include(lp => lp.User)
                .FirstOrDefaultAsync(lp => lp.Id == id && lp.Status == 1);
        }

        public async Task<List<LearningProgress>> GetAllAsync()
        {
            return await _context.LearningProgresses
                .Include(lp => lp.Question)
                .Include(lp => lp.User)
                .Where(lp => lp.Status == 1)
                .ToListAsync();
        }

        public async Task AddAsync(LearningProgress entity)
        {
            PrepareCreate(entity);
            await SaveAsync();
        }

        public async Task UpdateAsync(LearningProgress entity)
        {
            PrepareUpdate(entity);
            await SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var lp = await GetByIdAsync(id);
            if (lp != null)
            {
                lp.Status = 0;
                lp.UpdateAt = DateTime.UtcNow.ToLocalTime();
                PrepareUpdate(lp);
                await SaveAsync();
            }
        }

        public async Task<List<LearningProgress>> GetByUserAndQuestionAsync(Guid? userId, Guid? questionId)
        {
            var query = _context.LearningProgresses
                .Include(lp => lp.Question)
                .Include(lp => lp.User)
                .Where(lp => lp.Status == 1);

            if (userId.HasValue)
                query = query.Where(lp => lp.UserId == userId.Value);

            if (questionId.HasValue)
                query = query.Where(lp => lp.QuestionId == questionId.Value);

            return await query.ToListAsync();
        }
    }
}
