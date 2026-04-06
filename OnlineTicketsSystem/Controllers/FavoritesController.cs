
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using OnlineTicketsSystem.Services.Interfaces;

//namespace OnlineTicketsSystem.Controllers
//{
//    [Authorize]
//    public class FavoritesController : Controller
//    {
//        private readonly UserManager<IdentityUser> _userManager;
//        private readonly IFavoriteService _favoriteService;

//        public FavoritesController(UserManager<IdentityUser> userManager, IFavoriteService favoriteService)
//        {
//            _userManager = userManager;
//            _favoriteService = favoriteService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> My()
//        {
//            var userId = _userManager.GetUserId(User);
//            if (string.IsNullOrEmpty(userId)) return Challenge();

//            var favs = await _favoriteService.GetUserFavoritesAsync(userId);
//            return View(favs);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Add(int eventId, string? returnUrl = null)
//        {
//            var userId = _userManager.GetUserId(User);
//            if (string.IsNullOrEmpty(userId)) return Challenge();

//            TempData["Message"] = await _favoriteService.AddFavoriteAsync(userId, eventId);

//            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
//                return Redirect(returnUrl);

//            return RedirectToAction(nameof(My));
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Remove(int eventId, string? returnUrl = null)
//        {
//            var userId = _userManager.GetUserId(User);
//            if (string.IsNullOrEmpty(userId)) return Challenge();

//            TempData["Message"] = await _favoriteService.RemoveFavoriteAsync(userId, eventId);

//            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
//                return Redirect(returnUrl);

//            return RedirectToAction(nameof(My));
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Toggle(int eventId, string? returnUrl = null)
//        {
//            var userId = _userManager.GetUserId(User);
//            if (string.IsNullOrEmpty(userId)) return Challenge();

//            TempData["Message"] = await _favoriteService.ToggleFavoriteAsync(userId, eventId);

//            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
//                return Redirect(returnUrl);

//            return RedirectToAction(nameof(My));
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
    public class FavoritesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(UserManager<IdentityUser> userManager, IFavoriteService favoriteService)
        {
            _userManager = userManager;
            _favoriteService = favoriteService;
        }

        private string? GetUserId()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Warning"] = "Трябва да влезете в профила си.";
                return null;
            }

            return userId;
        }

        private IActionResult RedirectBack(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(My));
        }

        private void HandleMessage(string result)
        {
            if (result.Contains("Добавено"))
                TempData["Success"] = result;
            else if (result.Contains("Премахнато"))
                TempData["Info"] = result;
            else
                TempData["Error"] = result;
        }

        [HttpGet]
        public async Task<IActionResult> My()
        {
            var userId = GetUserId();
            if (userId == null) return Challenge();

            var favs = await _favoriteService.GetUserFavoritesAsync(userId);
            return View(favs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int eventId, string? returnUrl = null)
        {
            var userId = GetUserId();
            if (userId == null) return Challenge();

            var result = await _favoriteService.AddFavoriteAsync(userId, eventId);
            HandleMessage(result);

            return RedirectBack(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int eventId, string? returnUrl = null)
        {
            var userId = GetUserId();
            if (userId == null) return Challenge();

            var result = await _favoriteService.RemoveFavoriteAsync(userId, eventId);
            HandleMessage(result);

            return RedirectBack(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int eventId, string? returnUrl = null)
        {
            var userId = GetUserId();
            if (userId == null) return Challenge();

            var result = await _favoriteService.ToggleFavoriteAsync(userId, eventId);
            HandleMessage(result);

            return RedirectBack(returnUrl);
        }
    }
}

