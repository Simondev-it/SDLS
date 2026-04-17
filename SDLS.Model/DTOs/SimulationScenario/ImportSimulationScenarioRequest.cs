using Microsoft.AspNetCore.Http;

namespace SDLS.Model.DTOs.SimulationScenario
{
    public class ImportSimulationScenarioRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
