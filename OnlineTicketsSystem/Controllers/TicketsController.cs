using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Data;
namespace OnlineTicketsSystem.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TicketsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // /Tickets/My
        public async Task<IActionResult> My()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var tickets = await _context.Tickets
                .Include(t => t.Event)
                .ThenInclude(e => e.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.PaidAt ?? t.PurchaseDate)
                .ToListAsync();

            return View(tickets);
        }
    }
}
