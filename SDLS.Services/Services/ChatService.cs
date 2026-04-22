using Microsoft.Extensions.Configuration;
using SDLS.Model.DTOs.User;
using SDLS.Model.DTOs.UserLicense;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SDLS.Services.Services
{

    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserLicenseRepository _userLicenseRepo;
        private readonly IUserRepository _userRepo;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        private static DateTime _lastCallTime = DateTime.MinValue;

        public ChatService(
            IChatRepository chatRepository,
            IUserLicenseRepository userLicenseRepo,
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IUserRepository userRepo)
        {
            _chatRepository = chatRepository;
            _userLicenseRepo = userLicenseRepo;
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = config["Gemini:ApiKey"]
                ?? throw new Exception("Missing API Key");
            _userRepo = userRepo;
        }

        public string GetGreeting()
            => "Xin chào! Mình là trợ lý học lái xe 🚗";

        
        public async Task<(string Reply, string SessionId)> AskAsync(string prompt, string? userId)
        {
            string sessionId = userId ?? Guid.NewGuid().ToString();
            string cleanedPrompt = prompt.Trim();

            
            UserAIProfile? profile = null;

            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out Guid uid))
            {
                var user = await _userRepo.GetByIdAsync(uid);

                if (user != null)
                {
                    profile = new UserAIProfile
                    {
                        Id = user.Id,
                        Name = user.Name,
                        LicenseType = user.LicenseType
                    };
                }
            }

            string systemPrompt = SystemPromptBuilder.Build(profile);

            //  dùng history cho lộ trình → tránh lạc đề
            var history = IsLearningIntent(cleanedPrompt)
                ? new List<(string, string)>()
                : _chatRepository.GetHistory(sessionId);

            string context = BuildContext(systemPrompt, history, cleanedPrompt);

            string reply = await SendToGeminiAsync(context);

            _chatRepository.SaveToHistory(sessionId, cleanedPrompt, reply);

            return (reply, sessionId);
        }

        
        public async Task<string> AskExerciseAsync(string question)
        {
            string prompt = $"""
                Bạn là AI giải đề GPLX.

                ### ✅ Đáp án đúng:
                (A/B/C/D)

                ### 📌 Giải thích:
                - Ngắn gọn dễ hiểu

                ### ⚠️ Mẹo:
                - Nhớ nhanh

                Câu hỏi:
                {question}
                """;

            return await SendToGeminiAsync(prompt);
        }

        private static string BuildContext(
            string systemPrompt,
            IEnumerable<(string Question, string Answer)> history,
            string currentPrompt)
        {
            var sb = new StringBuilder();

            sb.AppendLine(systemPrompt);
            sb.AppendLine("\n-----------------\n");

            foreach (var (q, a) in history.TakeLast(3))
            {
                sb.AppendLine($"User: {q}");
                sb.AppendLine($"Assistant: {a}");
            }

            sb.AppendLine($"User: {currentPrompt}");
            sb.Append("Assistant:");

            return sb.ToString();
        }

        
        private static bool IsLearningIntent(string prompt)
        {
            var p = prompt.ToLower();

            return p.Contains("lộ trình")
                || p.Contains("kế hoạch")
                || p.Contains("học")
                || p.Contains("bao lâu")
                || p.Contains("mất bao lâu");
        }

        
        private async Task<string> SendToGeminiAsync(string prompt)
        {
            if (prompt.Length > 8000)
                prompt = prompt[..8000];

            // chống spam
            if ((DateTime.Now - _lastCallTime).TotalMilliseconds < 1000)
                return "⚠️ Bạn hỏi nhanh quá, chậm lại chút nhé!";

            _lastCallTime = DateTime.Now;

            int retry = 3;
            int delay = 1000;

            while (retry-- > 0)
            {
                var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={_apiKey}";

                var payload = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    using var doc = JsonDocument.Parse(result);

                    return doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString() ?? "AI không trả lời.";
                }

               
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                    response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    await Task.Delay(delay);
                    delay *= 2;
                    continue;
                }

                return $"⚠️ Lỗi AI: {response.StatusCode}";
            }

            return "⚠️ AI quá tải, thử lại sau!";
        }

        
        public void ClearSession(string sessionId)
        {
            _chatRepository.ClearHistory(sessionId);
        }
    }
}


//var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={_apiKey}";