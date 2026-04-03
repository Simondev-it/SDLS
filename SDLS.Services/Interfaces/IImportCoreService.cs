using Microsoft.AspNetCore.Http;

namespace SDLS.Services.Interfaces
{
    public interface IImportCoreService
    {
        Task<byte[]> BuildTemplateAsync(
            IReadOnlyList<string> headers,
            IReadOnlyList<string> sample,
            string format = "xlsx",
            string sheetName = "Template");

        Task<List<Dictionary<string, string>>> ReadRowsAsync(IFormFile file);
    }
}
