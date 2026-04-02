using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.Payment
{
    public class PayOSRequestModel
    {
        public long OrderId { get; set; }
        public int Amount { get; set; }
    }
}
