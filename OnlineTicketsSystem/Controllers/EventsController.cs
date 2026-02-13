using Microsoft.AspNetCore.Mvc;
using OnlineTicketsSystem.Data;
using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OnlineTicketsSystem.ViewModels;


namespace OnlineTicketsSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Events
        public async Task<IActionResult> Index(string category, string city, DateTime? date)
        {
            
            var list = await _context.Events
       .Include(e => e.Category)
       .OrderBy(e => e.Date)
       .ToListAsync();

            var today = DateTime.Today;

            var model = new EventsIndexViewModel
            {
                Upcoming = list.Where(e => e.Date.Date >= today).OrderBy(e => e.Date).ToList(),
                Past = list.Where(e => e.Date.Date < today).OrderByDescending(e => e.Date).ToList()
            };

            return View(model);
        }

       
        public async Task<IActionResult> Details(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return NotFound();

            // колко билета са купени за това събитие
            var soldTickets = await _context.Tickets.CountAsync(t => t.EventId == id);

            var remainingSeats = ev.Capacity - soldTickets;

            var model = new OnlineTicketsSystem.ViewModels.EventDetailsViewModel
            {
                Event = ev,
                SoldTickets = soldTickets,
                RemainingSeats = remainingSeats
            };

            return View(model);
        }



        [Authorize]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> BuyTicket(int id, int quantity = 1)
            {
                if (quantity < 1) quantity = 1;
                if (quantity > 10) quantity = 10; // лимит за сигурност (може да го махнеш)

                var ev = await _context.Events
                    .Include(e => e.Category)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (ev == null)
                    return NotFound();

                // Колко билета вече са купени за това събитие
                var sold = await _context.Tickets.CountAsync(t => t.EventId == id);

                // Свободни места
                var remaining = ev.Capacity - sold;

                if (remaining <= 0)
                {
                    TempData["Message"] = "Събитието е разпродадено. Няма свободни места.";
                    return RedirectToAction("Details", new { id });
                }

                if (quantity > remaining)
                {
                    TempData["Message"] = $"Няма достатъчно свободни места. Остават: {remaining}.";
                    return RedirectToAction("Details", new { id });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Добавяме quantity на брой билети (редове)
                for (int i = 0; i < quantity; i++)
                {
                    _context.Tickets.Add(new Ticket
                    {
                        EventId = id,
                        UserId = userId,
                        PurchaseDate = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();

                TempData["Message"] = $"Успешно закупихте {quantity} билет(а).";
                return RedirectToAction("Details", new { id });
            }

        }
    }

