using Microsoft.Extensions.Configuration;
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
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        private const string DefaultGreeting = "Xin chào! Tôi là trợ lý AI của bạn. Hãy đặt câu hỏi để tôi hỗ trợ nhé!";

        public ChatService(IChatRepository chatRepository, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _chatRepository = chatRepository;
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = config["Gemini:ApiKey"] ?? throw new Exception("Gemini:ApiKey chưa được cấu hình");
        }

        public string GetGreeting() => DefaultGreeting;

        public async Task<(string Reply, string SessionId)> AskAsync(string prompt, string? userIdentifier)
        {
            string sessionId = userIdentifier ?? Guid.NewGuid().ToString();
            string cleanedPrompt = prompt.Trim();

            var history = _chatRepository.GetHistory(sessionId);
            string context = BuildContext(history, cleanedPrompt);

            string reply = await SendToGeminiAsync(context);

            _chatRepository.SaveToHistory(sessionId, cleanedPrompt, reply);

            return (reply, sessionId);
        }

        private static string BuildContext(IEnumerable<(string Question, string Answer)> history, string currentPrompt)
        {
            var sb = new StringBuilder();

            foreach (var (q, a) in history.TakeLast(10))
            {
                sb.AppendLine($"Người dùng: {q}");
                sb.AppendLine($"Trợ lý: {a}");
            }

            sb.AppendLine($"Người dùng: {currentPrompt}");
            sb.Append("Trợ lý: ");

            return sb.ToString();
        }

        private async Task<string> SendToGeminiAsync(string prompt)
        {
            var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var payload = new
            {
                contents = new[]
                {
                new { parts = new[] { new { text = prompt } } }
            }
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return $"⚠️ Lỗi AI: {response.StatusCode}";

            using var doc = JsonDocument.Parse(result);
            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "AI không trả lời.";
        }
    }
}
