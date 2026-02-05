using Microsoft.AspNetCore.Mvc;
namespace OnlineTicketsSystem.Controllers
{
    [Route("Cart")]
    public class CartController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            // Засега само да видим, че работи
            return Content("Кошницата работи ✅");
        }

        [HttpPost("Add")]
        public IActionResult Add(int eventId, int quantity = 1)
        {
            // Засега само тест
            return RedirectToAction("Index");
        }
    }
}

