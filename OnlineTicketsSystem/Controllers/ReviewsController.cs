

//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using OnlineTicketsSystem.Services.Interfaces;

//namespace OnlineTicketsSystem.Controllers
//{
//    [Authorize]
//    public class ReviewsController : Controller
//    {
//        private readonly UserManager<IdentityUser> _userManager;
//        private readonly IReviewService _reviewService;

//        public ReviewsController(UserManager<IdentityUser> userManager, IReviewService reviewService)
//        {
//            _userManager = userManager;
//            _reviewService = reviewService;
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(int eventId, int rating, string? comment)
//        {
//            var userId = _userManager.GetUserId(User);
//            if (string.IsNullOrEmpty(userId)) return Challenge();

//            TempData["Message"] = await _reviewService.CreateReviewAsync(userId, eventId, rating, comment);
//            return RedirectToAction("Details", "Events", new { id = eventId });
//        }
//    }
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineTicketsSystem.Services.Interfaces;

namespace OnlineTicketsSystem.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IReviewService _reviewService;

        public ReviewsController(UserManager<IdentityUser> userManager, IReviewService reviewService)
        {
            _userManager = userManager;
            _reviewService = reviewService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int eventId, int rating, string? comment)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Warning"] = "Трябва да влезете в профила си, за да оставите отзив.";
                return Challenge();
            }

            var result = await _reviewService.CreateReviewAsync(userId, eventId, rating, comment);

            if (result == "OK")
            {
                TempData["Success"] = "Вашето отзив беше добавено успешно!";
            }
            else
            {
                TempData["Error"] = result ?? "Възникна грешка при добавянето на отзив.";
            }

            return RedirectToAction("Details", "Events", new { id = eventId });
        }
    }
}

