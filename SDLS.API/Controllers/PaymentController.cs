using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs.Payment;
using SDLS.Services.Interfaces;
using System.Text.Json;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/payos")]
    public class PaymentController : ControllerBase
    {
        private readonly IPayOSService _payOSService;

        public PaymentController(IPayOSService payOSService)
        {
            _payOSService = payOSService;
        }

        // 🔥 CREATE PAYMENT
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromQuery] Guid userId, [FromBody] PayOSRequestModel model)
        {
            try
            {
                var result = await _payOSService.CreatePayment(userId, model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔥 WEBHOOK
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] JsonElement payload)
        {
            try
            {
                await _payOSService.HandleWebhook(payload);
                return Ok(new { message = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllPayments()
        {
            var result = await _payOSService.GetAllPaymentsAsync();
            return Ok(result);
        }
    }
}
