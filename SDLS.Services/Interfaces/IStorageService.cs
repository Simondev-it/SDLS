using Microsoft.AspNetCore.Http;
using SDLS.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Services.Interfaces
{
    public interface IStorageService
    {
        Task<string> UploadImageAsync(IFormFile file, ImageTarget target, Guid entityId);
        Task<bool> DeleteImageAsync(string fileUrl, ImageTarget target);
    }
}
