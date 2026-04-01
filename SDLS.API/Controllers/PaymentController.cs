using Microsoft.AspNetCore.Mvc;

namespace SDLS.API.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
