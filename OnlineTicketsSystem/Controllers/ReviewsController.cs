using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Data;
using OnlineTicketsSystem.Models;

namespace OnlineTicketsSystem.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int eventId, int rating, string? comment)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            var evExists = await _context.Events.AnyAsync(e => e.Id == eventId);
            if (!evExists) return NotFound();

            // ✅ Само с платен билет
            var hasPaidTicket = await _context.Tickets.AnyAsync(t =>
                t.UserId == userId &&
                t.EventId == eventId &&
                t.IsPaid);

            if (!hasPaidTicket)
            {
                TempData["Message"] = "Можеш да оставиш отзив само ако имаш платен билет за това събитие.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            // ✅ Само 1 отзив
            var already = await _context.Reviews
                .IgnoreQueryFilters()
                .AnyAsync(r => r.UserId == userId && r.EventId == eventId && !r.IsDeleted);

            if (already)
            {
                TempData["Message"] = "Вече си оставил/а отзив за това събитие.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            if (rating < 1) rating = 1;
            if (rating > 5) rating = 5;

            var review = new Review
            {
                UserId = userId,
                EventId = eventId,
                Rating = rating,
                Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsApproved = true // ако искаш админ одобрение => false
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Благодарим! Отзивът е добавен.";
            return RedirectToAction("Details", "Events", new { id = eventId });
        }

    }
}
