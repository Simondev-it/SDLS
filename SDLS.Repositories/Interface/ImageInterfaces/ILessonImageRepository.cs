using SDLS.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Repositories.Interface.ImageInterfaces
{
    public interface ILessonImageRepository
    {
        Task<LessonImage> GetByIdAsync();
        Task<LessonImage> GetByLessonIdAsync();
        Task<IEnumerable<LessonImage>> GetAllAsync();
        Task AddAsync(LessonImage image);
        Task DeleteAsync(Guid id);
    }
}
