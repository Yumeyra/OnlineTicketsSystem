using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace OnlineTicketsSystem.Controllers
{
    [Route("Cart")]
    [Authorize]
    public class CartController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            // Засега само да видим, че работи
            return Content("Кошницата работи ✅");
        }


        [Authorize]
        [HttpPost]
        public IActionResult Add(int eventId, int quantity)
        {
            if (quantity < 1) quantity = 1;
            if (quantity > 20) quantity = 20;

            // TODO: тук добавяш в кошницата (session/db)
            // AddToCart(eventId, quantity);

            TempData["Message"] = $"Успешно закупихте {quantity} билет(а).";
            return RedirectToAction("Details", "Events", new { id = eventId });
        }
    }
}

