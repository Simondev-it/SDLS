using Microsoft.EntityFrameworkCore;
using SDLS.Model.Helpers;
using SDLS.Model.Models;
using SDLS.Repositories.Base;
using SDLS.Repositories.Helper;
using SDLS.Repositories.Interface;

namespace SDLS.Repositories.Repositories
{
    public class SimulationScenarioRepository : GenericRepository<SimulationScenario>, ISimulationScenarioRepository
    {
        public async Task<IEnumerable<SimulationScenario>> GetAllAsync(
            Guid? simulationCategoryId = null,
            Guid? simulationChapterId = null,
            Guid? simulationDifficultyLevelId = null,
            string? name = null,
            int? status = null,
            string? role = null)
        {
            var query = _context.SimulationScenarios
                .Include(x => x.SimulationCategory)
                .Include(x => x.SimulationChapter)
                .Include(x => x.SimulationDifficultyLevel)
                .AsQueryable();

            if (simulationCategoryId.HasValue)
                query = query.Where(x => x.SimulationCategoryId == simulationCategoryId.Value);

            if (simulationChapterId.HasValue)
                query = query.Where(x => x.SimulationChapterId == simulationChapterId.Value);

            if (simulationDifficultyLevelId.HasValue)
                query = query.Where(x => x.SimulationDifficultyLevelId == simulationDifficultyLevelId.Value);

            if (!string.IsNullOrWhiteSpace(name))
            {
                var keyword = name.Trim();
                query = query.Where(x => x.Name != null && EF.Functions.ILike(x.Name, $"%{keyword}%"));
            }

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            query = query.ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
            {
                query = query.Where(x =>
                    (x.SimulationCategory == null || x.SimulationCategory.Status != 0) &&
                    (x.SimulationChapter == null || x.SimulationChapter.Status != 0) &&
                    (x.SimulationDifficultyLevel == null || x.SimulationDifficultyLevel.Status != 0));
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<SimulationScenario?> GetByIdAsync(Guid id, string? role = null)
        {
            var query = _context.SimulationScenarios
                .Include(x => x.SimulationCategory)
                .Include(x => x.SimulationChapter)
                .Include(x => x.SimulationDifficultyLevel)
                .Where(x => x.Id == id)
                .ApplyRoleFilter(role);

            if (!QueryableRoleFilterExtensions.IsPrivilegedRole(role))
            {
                query = query.Where(x =>
                    (x.SimulationCategory == null || x.SimulationCategory.Status != 0) &&
                    (x.SimulationChapter == null || x.SimulationChapter.Status != 0) &&
                    (x.SimulationDifficultyLevel == null || x.SimulationDifficultyLevel.Status != 0));
            }

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<SimulationScenario?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.SimulationScenarios.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(SimulationScenario entity)
        {
            await _context.SimulationScenarios.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SimulationScenario entity)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftAsync(Guid id)
        {
            var existing = await _context.SimulationScenarios.FirstOrDefaultAsync(x => x.Id == id && x.Status == 1);
            if (existing == null) return;

            existing.Status = 0;
            existing.UpdateAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHardAsync(Guid id)
        {
            var existing = await _context.SimulationScenarios.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return;

            _context.SimulationScenarios.Remove(existing);
            await _context.SaveChangesAsync();
        }

        public async Task<double> CalculateDurationAsync(List<Guid> scenarioIds)
        {
            if (scenarioIds.Count == 0)
                return 0d;

            var scenarios = await _context.SimulationScenarios
                .Where(x => scenarioIds.Contains(x.Id) && x.Status == 1)
                .Select(x => new { x.Id, x.TotalTime })
                .ToListAsync();

            if (scenarios.Count != scenarioIds.Count)
                throw new KeyNotFoundException("Có SimulationScenario không tồn tại hoặc không active.");

            return scenarios.Sum(x => x.TotalTime);
        }
    }
}