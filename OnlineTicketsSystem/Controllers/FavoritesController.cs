using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Data;
using OnlineTicketsSystem.Models;
using OnlineTicketsSystem.ViewModels;
using System.Security.Claims;
namespace OnlineTicketsSystem.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FavoritesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Favorites/My
        [HttpGet]
        public async Task<IActionResult> My()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            // Понеже имаш QueryFilter за Favorite (!IsDeleted),
            // тук автоматично идват само НЕ-изтритите.
            var favs = await _context.Favorites
                .Include(f => f.Event)
                    .ThenInclude(e => e.Category)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return View(favs);
        }

        // POST: /Favorites/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int eventId, string? returnUrl = null)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            // проверка дали event съществува (с query filter за Event)
            var exists = await _context.Events.AnyAsync(e => e.Id == eventId);
            if (!exists) return NotFound();

            // ВНИМАНИЕ: понеже Favorites имат QueryFilter (!IsDeleted),
            // ако търсим "existing" само в Favorites, няма да намерим soft-deleted.
            // Затова: IgnoreQueryFilters()
            var existing = await _context.Favorites
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.EventId == eventId);

            if (existing == null)
            {
                _context.Favorites.Add(new Favorite
                {
                    UserId = userId,
                    EventId = eventId,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    DeletedAt = null
                });

                TempData["Message"] = "Добавено в любими.";
            }
            else
            {
                if (existing.IsDeleted)
                {
                    existing.IsDeleted = false;
                    existing.DeletedAt = null;
                    TempData["Message"] = "Добавено в любими.";
                }
                else
                {
                    TempData["Message"] = "Това събитие вече е в любими.";
                }
            }

            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(My));
        }

        // POST: /Favorites/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int eventId, string? returnUrl = null)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            // Тук може и без IgnoreQueryFilters, защото махаме само активни
            var fav = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.EventId == eventId);

            if (fav == null)
            {
                TempData["Message"] = "Не е намерено в любими.";
            }
            else
            {
                // Soft delete (съответства на твоя модел)
                fav.IsDeleted = true;
                fav.DeletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["Message"] = "Премахнато от любими.";
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(My));
        }

        // POST: /Favorites/Toggle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int eventId, string? returnUrl = null)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var exists = await _context.Events.AnyAsync(e => e.Id == eventId);
            if (!exists) return NotFound();

            var fav = await _context.Favorites
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.EventId == eventId);

            if (fav == null)
            {
                _context.Favorites.Add(new Favorite
                {
                    UserId = userId,
                    EventId = eventId,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
                TempData["Message"] = "Добавено в любими.";
            }
            else
            {
                if (fav.IsDeleted)
                {
                    fav.IsDeleted = false;
                    fav.DeletedAt = null;
                    TempData["Message"] = "Добавено в любими.";
                }
                else
                {
                    fav.IsDeleted = true;
                    fav.DeletedAt = DateTime.UtcNow;
                    TempData["Message"] = "Премахнато от любими.";
                }
            }

            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(My));
        }
    }
}

