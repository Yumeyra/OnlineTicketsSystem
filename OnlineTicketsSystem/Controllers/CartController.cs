

//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using OnlineTicketsSystem.Services.Interfaces;

//namespace OnlineTicketsSystem.Controllers
//{
//    public class CartController : Controller
//    {
//        private readonly ICartService _cartService;
//        private readonly UserManager<IdentityUser> _userManager;

//        public CartController(ICartService cartService, UserManager<IdentityUser> userManager)
//        {
//            _cartService = cartService;
//            _userManager = userManager;
//        }

//        public IActionResult Index()
//        {
//            var cart = _cartService.GetCart(HttpContext.Session);
//            return View(cart);
//        }

//        [Authorize]
//        [HttpPost]
//        public async Task<IActionResult> Add(int eventId, int quantity)
//        {
//            var userId = _userManager.GetUserId(User);
//            if (userId == null) return Challenge();

//            var error = await _cartService.AddToCartAsync(
//                HttpContext.Session, userId, eventId, quantity);

//            if (error != null)
//                TempData["Message"] = error;
//            else
//                TempData["Message"] = "Добавено в кошницата.";

//            return RedirectToAction("Index");
//        }

//        [Authorize]
//        [HttpPost]
//        public async Task<IActionResult> Remove(int eventId)
//        {
//            var userId = _userManager.GetUserId(User);
//            await _cartService.RemoveFromCartAsync(
//                HttpContext.Session, userId!, eventId);

//            TempData["Message"] = "Премахнато.";
//            return RedirectToAction("Index");
//        }

//        [Authorize]
//        [HttpPost]
//        public async Task<IActionResult> Clear()
//        {
//            var userId = _userManager.GetUserId(User);
//            await _cartService.ClearCartAsync(
//                HttpContext.Session, userId!);

//            TempData["Message"] = "Кошницата е изчистена.";
//            return RedirectToAction("Index");
//        }

//        [Authorize]
//        [HttpPost]
//        public async Task<IActionResult> Checkout()
//        {
//            var userId = _userManager.GetUserId(User);
//            if (userId == null) return Challenge();

//            var error = await _cartService.CheckoutAsync(
//                HttpContext.Session, userId);

//            if (error != null)
//            {
//                TempData["Message"] = error;
//                return RedirectToAction("Index");
//            }

//            TempData["Message"] = "Успешно плащане!";
//            return RedirectToAction("My", "Tickets");
//        }
//    }
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineTicketsSystem.Services.Interfaces;

namespace OnlineTicketsSystem.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(ICartService cartService, UserManager<IdentityUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var cart = _cartService.GetCart(HttpContext.Session);
            return View(cart);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(int eventId, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                TempData["Warning"] = "Трябва да влезете в профила си.";
                return Challenge();
            }

            var error = await _cartService.AddToCartAsync(
                HttpContext.Session, userId, eventId, quantity);

            if (error != null)
                TempData["Error"] = error;
            else
                TempData["Success"] = "Добавено в кошницата.";

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Remove(int eventId)
        {
            var userId = _userManager.GetUserId(User);

            await _cartService.RemoveFromCartAsync(
                HttpContext.Session, userId!, eventId);

            TempData["Success"] = "Премахнато от кошницата.";
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            var userId = _userManager.GetUserId(User);

            await _cartService.ClearCartAsync(
                HttpContext.Session, userId!);

            TempData["Info"] = "Кошницата е изчистена.";
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                TempData["Warning"] = "Трябва да влезете в профила си.";
                return Challenge();
            }

            var error = await _cartService.CheckoutAsync(
                HttpContext.Session, userId);

            if (error != null)
            {
                TempData["Error"] = error;
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Успешно плащане!";
            return RedirectToAction("My", "Tickets");
        }
    }
}
