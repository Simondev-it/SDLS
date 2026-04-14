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
        Task<LessonImage> GetByIdAsync(Guid Id);
        Task<IEnumerable<LessonImage>> GetByLessonIdAsync(Guid Id);
        Task<IEnumerable<LessonImage>> GetAllAsync();
        Task AddAsync(LessonImage image);
        Task DeleteAsync(Guid id);
    }
}
