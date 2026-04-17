using Microsoft.AspNetCore.Http;

namespace SDLS.Model.DTOs.Tag
{
    public class ImportTagRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
