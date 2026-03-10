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
        Task<QuestionDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<QuestionDTO>> GetAllAsync();
        Task<QuestionDTO> CreateAsync(QuestionCreateDTO dto);
        Task<QuestionDTO> UpdateAsync(Guid id, QuestionUpdateDTO dto);
        Task DeleteAsync(Guid id);
    }
}
