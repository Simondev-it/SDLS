using SDLS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Repositories.Interface.ImageInterfaces
{
    public interface IPostImageRepository
    {
        Task<IPostImageRepository> GetPostImageByIdAsync();
        Task<IEnumerable<PostImage>> GetAllAsync();
        Task AddAsync(PostImage image);
        Task DeleteAsync(Guid id);
    }
}
