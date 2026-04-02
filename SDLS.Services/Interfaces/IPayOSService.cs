using SDLS.Model.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SDLS.Services.Interfaces
{
    public interface IPayOSService
    {
        
        Task<object> CreatePayment(Guid userId, PayOSRequestModel model);

        Task HandleWebhook(JsonElement payload);
    }
}
