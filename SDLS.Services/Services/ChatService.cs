using Microsoft.Extensions.Configuration;
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
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        private static DateTime _lastCallTime = DateTime.MinValue;

        public ChatService(
            IChatRepository chatRepository,
            IUserLicenseRepository userLicenseRepo,
            IHttpClientFactory httpClientFactory,
            IConfiguration config)
        {
            _chatRepository = chatRepository;
            _userLicenseRepo = userLicenseRepo;
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = config["Gemini:ApiKey"]
                ?? throw new Exception("Missing API Key");
        }

        public string GetGreeting()
            => "Xin chào! Mình là trợ lý học lái xe 🚗";

        // =========================
        // 🧠 CHAT THÔNG MINH
        // =========================
        public async Task<(string Reply, string SessionId)> AskAsync(string prompt, string? userId)
        {
            string sessionId = userId ?? Guid.NewGuid().ToString(); // ✅ mỗi request 1 session (hoặc FE giữ)
            string cleanedPrompt = prompt.Trim();

            // 🔥 FIX GUID an toàn
            UserLicenseDTO? userLicense = null;

            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out Guid parsedId))
            {
                userLicense = await _userLicenseRepo.GetByUserIdAsync(parsedId);
            }

            string systemPrompt = SystemPromptBuilder.Build(userLicense);

            var history = _chatRepository.GetHistory(sessionId);

            // 🔥 FIX lạc đề
            if (IsNewIntent(cleanedPrompt))
            {
                history = new List<(string, string)>();
            }

            string context = BuildContext(systemPrompt, history, cleanedPrompt);

            string reply = await SendToGeminiAsync(context);

            _chatRepository.SaveToHistory(sessionId, cleanedPrompt, reply);

            return (reply, sessionId);
        }


        public async Task<string> AskExerciseAsync(string question)
        {
            string prompt = BuildExercisePrompt(question);
            return await SendToGeminiAsync(prompt);
        }

        // =========================
        // 🧠 BUILD CONTEXT
        // =========================
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

        // =========================
        // 🔥 INTENT DETECT
        // =========================
        private static bool IsNewIntent(string prompt)
        {
            var p = prompt.ToLower();

            return p.Contains("lộ trình")
                || p.Contains("kế hoạch")
                || p.Contains("học")
                || p.Contains("thi")
                || p.Contains("sa hình")
                || p.Contains("mẹo")
                || p.Contains("bao lâu")
                || p.Contains("như nào");
        }

        // =========================
        // 🔥 PROMPT GIẢI BÀI
        // =========================
        private static string BuildExercisePrompt(string question)
        {
            return $"""
                Bạn là AI giải đề GPLX.

                ### ✅ Đáp án đúng:
                (A/B/C/D)

                ###📌 Giải thích:
                - Ngắn gọn

                ### ⚠️ Mẹo:
                - Dễ nhớ

                Câu hỏi:
                {question}
                """;
        }

        // =========================
        // 🌐 CALL AI
        // =========================
        private async Task<string> SendToGeminiAsync(string prompt)
        {
            if (prompt.Length > 8000)
                prompt = prompt.Substring(0, 8000);

            // 🔥 Anti spam backend (cực quan trọng)
            if ((DateTime.Now - _lastCallTime).TotalMilliseconds < 800)
            {
                return "⚠️ Bạn gửi câu hỏi quá nhanh, hãy chờ 1 chút nhé!";
            }

            _lastCallTime = DateTime.Now;

            int retry = 4;
            int delay = 1000; // 1s

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

                // 🔥 FIX 429 (Too Many Requests)
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    await Task.Delay(delay);
                    delay *= 2; // exponential backoff
                    continue;
                }

                // 🔥 FIX 503
                if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    await Task.Delay(delay);
                    delay *= 2;
                    continue;
                }

                return $"⚠️ Lỗi AI: {response.StatusCode}";
            }

            return BuildFallbackAnswer(prompt);
        }




        private string BuildFallbackAnswer(string prompt)
        {
            if (prompt.ToLower().Contains("lộ trình"))
            {
                return """
                ⚠️ AI đang bận, mình gợi ý nhanh:

                ### 🚗 Lộ trình cơ bản
                1. Học lý thuyết (1-2 tuần)
                2. Làm 600 câu hỏi
                3. Luyện sa hình
                4. Ôn thi

                📌 Khi hệ thống ổn định, mình sẽ tư vấn chi tiết hơn!
                """;
                            }

                             if (prompt.ToLower().Contains("sa hình"))
                            {
                               return """
                ⚠️ AI đang bận, mẹo nhanh:

                ### 📌 Sa hình
                - Đi chậm, đều ga
                - Nhớ điểm canh
                - Không vội

                ✅ Luyện nhiều sẽ quen
                """;
            }

            return "⚠️ Hệ thống đang bận, thử lại sau vài giây nhé!";
        }

        // =========================
        // 🧹 CLEAR SESSION
        // =========================
        public void ClearSession(string sessionId)
        {
            _chatRepository.ClearHistory(sessionId);
        }
    }
}


//var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={_apiKey}";