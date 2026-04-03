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
    public class SimulationExamRepository : GenericRepository<SimulationExam>, ISimulationExamRepository
    {
        public async Task ValidateSimulationExamIdsAsync(Guid situationExamId, List<Guid> simulationExamIds)
        {
            if (simulationExamIds.Count == 0)
                throw new ArgumentException("SimulationSessionDetails không hợp lệ.");

            var validIds = await _context.SimulationExams
                .Where(x => x.Status == 1
                    && x.SituationExamId == situationExamId
                    && simulationExamIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            if (validIds.Count != simulationExamIds.Count)
                throw new KeyNotFoundException("Có SimulationExam không tồn tại, không active hoặc không thuộc SituationExam.");
        }
    }
}
