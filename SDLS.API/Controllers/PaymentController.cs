using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDLS.Model.DTOs.Payment;
using SDLS.Services.Interfaces;
using System.Text.Json;

namespace SDLS.API.Controllers
{
    [ApiController]
    [Route("api/Payos")]
    public class PaymentController : ControllerBase
    {
        private readonly IPayOSService _payOSService;

        public PaymentController(IPayOSService payOSService)
        {
            _payOSService = payOSService;
        }

        [Authorize]
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

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] JsonElement payload)
        {
            try
            {
                await _payOSService.HandleWebhook(payload);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("Webhook error: " + ex.Message);
            }

            
            return Ok(new { message = "received" });
        }
        [Authorize]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllPayments()
        {
            var result = await _payOSService.GetAllPaymentsAsync();
            return Ok(result);
        }
    }
}
