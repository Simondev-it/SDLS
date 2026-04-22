using PayOS.Models.V2.PaymentRequests;
using PayOS.Models;
using PayOS;
using SDLS.Model.DTOs.Payment;
using SDLS.Model.Models;
using SDLS.Repositories.Interface;
using SDLS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SDLS.Services.ApiExceptions;


namespace SDLS.Services.Services
{
    public class PayOSService : IPayOSService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly PayOSClient _client;

        public PayOSService(
            IPaymentRepository paymentRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            PayOSClient client)
        {
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _client = client;
        }

        // 🔥 CREATE PAYMENT
        public async Task<object> CreatePayment(Guid userId, PayOSRequestModel model)
        {
            // 1️⃣ check pending theo user
            var pending = await _paymentRepository.GetPendingByUserIdAsync(userId);
            if (pending != null)
            {
                pending.Status = -1; // Cancelled

                pending.UpdateAt = DateTime.SpecifyKind(
                    DateTime.UtcNow,
                    DateTimeKind.Unspecified
                );

                await _paymentRepository.UpdateAsync(pending);



                var newPending = await _paymentRepository.GetPendingByUserIdAsync(userId);
                if (newPending != null)
                    throw ApiException.Conflict("Có lỗi xảy ra, vui lòng thử lại");
            }
            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // 2️⃣ lưu DB
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderCode = orderCode,
                Amount = model.Amount,
                Method = "PayOS",
                Status = 0,
                Response = "not paid",
                Note = $"Membership Payment - {orderCode}",
                CreateAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
            };

            await _paymentRepository.AddAsync(payment);

            // 3️⃣ items
            var items = new[]
            {
            new { name = "Thanh toán membership", quantity = 1, price = model.Amount }
        };

            var requestData = new
            {
                orderCode,
                amount = model.Amount,
                description = $"OC:{orderCode}", 
                cancelUrl = "https://green-light-app.vercel.app/?message=Thanh%20toán%20thất%20bại",
                returnUrl = "https://green-light-app.vercel.app/?message=Thanh%20toán%20thành%20công",
                items
            };

            string signature = CreateSignature(requestData, _client.ChecksumKey);

            var finalRequest = new
            {
                requestData.orderCode,
                requestData.amount,
                requestData.description,
                requestData.cancelUrl,
                requestData.returnUrl,
                requestData.items,
                signature
            };

            var requestOptions = new RequestOptions<object>
            {
                Body = finalRequest
            };

            var response = await _client.PostAsync<CreatePaymentLinkResponse, object>(
                "/v2/payment-requests",
                requestOptions
            );

            return new
            {
                PaymentUrl = response.CheckoutUrl,
                OrderCode = orderCode
            };
        }

        // 🔥 WEBHOOK
        public async Task HandleWebhook(JsonElement payload)
        {
            var data = payload.GetProperty("data");
            string signature = payload.GetProperty("signature").GetString();

            var computed = CreateWebhookSignature(data, _client.ChecksumKey);

            if (computed != signature)
            {
                Console.WriteLine("Invalid signature");
                return; // ❌ KHÔNG throw
            }

            bool success = payload.GetProperty("success").GetBoolean();
            if (!success) return;

            long orderCode = data.GetProperty("orderCode").GetInt64();

            var payment = await _paymentRepository.GetByOrderCodeAsync(orderCode);
            if (payment == null) return;

            if (payment.Status == 1) return;

            // ✅ update payment
            payment.Status = 1;
            payment.Response = "Paid";
            await _paymentRepository.UpdateAsync(payment);

            // 🔥 upgrade role
            await UpgradeUserRole(payment.UserId);
        }

        // 🔥 UPGRADE ROLE
        private async Task UpgradeUserRole(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw ApiException.NotFound("User không tồn tại");

            // 🔥 RoleId của bạn
            var guestRoleId = Guid.Parse("67b62c82-e459-4b1a-b912-e1758a5c87c4");
            var userRoleId = Guid.Parse("66efa899-db8f-4866-bfe9-6fe9d98eca7e");

            // ✅ chỉ update nếu đang là Guest
            if (user.RoleId != guestRoleId)
                return;

            // 🔥 update role
            user.RoleId = userRoleId;
            user.UpdateAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await _userRepository.UpdateAsync(user);
        }

        // 🔥 SIGNATURE CREATE
        private string CreateSignature(object data, string key)
        {
            var dict = new Dictionary<string, string>();

            foreach (var prop in data.GetType().GetProperties())
            {
                var value = prop.GetValue(data);
                if (value == null || prop.Name == "items") continue;

                dict.Add(prop.Name, value.ToString());
            }

            var ordered = dict.OrderBy(x => x.Key);
            var raw = string.Join("&", ordered.Select(x => $"{x.Key}={x.Value}"));

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(raw));

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        // 🔥 SIGNATURE WEBHOOK
        private string CreateWebhookSignature(JsonElement data, string key)
        {
            var dict = new SortedDictionary<string, string>();

            foreach (var prop in data.EnumerateObject())
            {
                dict[prop.Name] = prop.Value.ToString();
            }

            var raw = string.Join("&", dict.Select(x => $"{x.Key}={x.Value}"));

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(raw));

            return Convert.ToHexString(hash).ToLower();
        }
        public async Task<List<PaymentDTO>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllPaymentsAsync();

            return payments.Select(p => new PaymentDTO
            {
                Id = p.Id,
                UserId = p.UserId,
                OrderCode = p.OrderCode,
                Method = p.Method,
                Amount = p.Amount,
                Note = p.Note,
                Response = p.Response,
                CreateAt = p.CreateAt,
                Status = p.Status
            }).ToList();
        }
    }
}
