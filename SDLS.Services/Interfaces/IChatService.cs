using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Services.Interfaces
{
    public interface IChatService
    {
        string GetGreeting();

        Task<(string Reply, string SessionId)> AskAsync(string prompt, string userId);

        Task<string> AskExerciseAsync(string question);

        void ClearSession(string sessionId);

    }
}
