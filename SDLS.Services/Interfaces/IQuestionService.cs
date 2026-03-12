using SDLS.Model.DTOs.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionDTO>> GetAllAsync();
        Task<QuestionDTO> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(QuestionCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, QuestionCreateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
