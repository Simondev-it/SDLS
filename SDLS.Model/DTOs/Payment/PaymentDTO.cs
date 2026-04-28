using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.Payment
{
    public class PaymentDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public long? OrderCode { get; set; }
        public string? Method { get; set; }
        public int? Amount { get; set; }
        public string? Note { get; set; }
        public string? Response { get; set; }
        public DateTime? CreateAt { get; set; }
        public int? Status { get; set; }

        // optional
        public string? UserName { get; set; }
    }
}
