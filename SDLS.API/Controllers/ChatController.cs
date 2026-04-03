using Microsoft.AspNetCore.Mvc;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;

namespace SDLS.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("greeting")]
        public IActionResult GetGreeting() => Ok(new { message = _chatService.GetGreeting() });

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest(new { message = "Prompt không được để trống" });

            var (reply, sessionId) = await _chatService.AskAsync(request.Prompt, request.UserIdentifier);

            return Ok(new { reply, sessionId });
        }

        [HttpGet("history/{sessionId}")]
        public IActionResult GetHistory(string sessionId, [FromServices] IChatRepository repo)
        {
            var history = repo.GetHistory(sessionId);
            return Ok(history.Select(h => new { question = h.Question, answer = h.Answer }));
        }
    }

    public record ChatRequest(string Prompt, string? UserIdentifier);
}
