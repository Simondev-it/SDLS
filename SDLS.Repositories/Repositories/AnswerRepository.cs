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
    public class AnswerRepository : GenericRepository<Answer>, IAnswerRepository
    {
        public async Task AddAsync(Answer answer)
        {
            this.Create(answer);
        }

        public async Task UpdateAsync(Answer answer)
        {
            this.Update(answer);
        }

        public async Task DeleteAsync(Guid id)
        {
            this.Remove(this.GetById(id));
        }
    }
}
