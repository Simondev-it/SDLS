using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Repositories.Interface
{
    public interface IChatRepository
    {
        List<(string Question, string Answer)> GetHistory(string sessionId);
        void SaveToHistory(string sessionId, string question, string answer);
        void ClearHistory(string sessionId);
    }
}
