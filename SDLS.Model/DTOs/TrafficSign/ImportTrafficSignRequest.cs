using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.TrafficSign
{
    public class ImportTrafficSignRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
