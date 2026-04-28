using Microsoft.Extensions.Caching.Memory;
using SDLS.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Repositories.Repositories
{

    public class ChatRepository : IChatRepository
    {
        private readonly IMemoryCache _memoryCache;
        private const string HistoryPrefix = "ChatHistory_";

        public ChatRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public List<(string Question, string Answer)> GetHistory(string sessionId)
        {
            return _memoryCache.Get<List<(string, string)>>(HistoryPrefix + sessionId)
                   ?? new List<(string, string)>();
        }

        public void SaveToHistory(string sessionId, string question, string answer)
        {
            var key = HistoryPrefix + sessionId;

            var history = _memoryCache.Get<List<(string, string)>>(key)
                          ?? new List<(string, string)>();

            history.Add((question, answer));

            _memoryCache.Set(key, history, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(30) //thêm để tối ưu
            });
        }

        public void ClearHistory(string sessionId)
        {
            _memoryCache.Remove(HistoryPrefix + sessionId);
        }
    }
}
