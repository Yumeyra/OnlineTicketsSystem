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
    public class EventsController : Controller
    {
        //private readonly ApplicationDbContext _context;

        //public EventsController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}


        //public async Task<IActionResult> Index(string? category, string? city, DateTime? date)
        //{
        //    var query = _context.Events
        //        .Include(e => e.Category)
        //        .AsQueryable();

        //    if (!string.IsNullOrWhiteSpace(category))
        //    {
        //        var cat = category.Trim();
        //        query = query.Where(e => e.Category != null && e.Category.Name == cat);
        //    }

        //    if (!string.IsNullOrWhiteSpace(city))
        //    {
        //        var c = city.Trim();
        //        query = query.Where(e => e.City.Contains(c));
        //    }

        //    if (date.HasValue)
        //    {
        //        var d = date.Value.Date;
        //        query = query.Where(e => e.Date.Date == d);
        //    }

        //    var all = await query
        //        .OrderBy(e => e.Date)
        //        .ToListAsync();

        //    var today = DateTime.Today;

        //    var vm = new EventsIndexViewModel
        //    {
        //        Upcoming = all.Where(e => e.Date.Date >= today).ToList(),
        //        Past = all.Where(e => e.Date.Date < today).ToList()
        //    };

        //    return View(vm);
        //}



        //public async Task<IActionResult> Details(int id)
        //{
        //    var ev = await _context.Events
        //        .Include(e => e.Category)
        //        .FirstOrDefaultAsync(e => e.Id == id);

        //    if (ev == null) return NotFound();

        //    // колко билета са купени за това събитие
        //    var soldTickets = await _context.Tickets.CountAsync(t => t.EventId == id);

        //    var remainingSeats = ev.Capacity - soldTickets;

        //    var model = new OnlineTicketsSystem.ViewModels.EventDetailsViewModel
        //    {
        //        Event = ev,
        //        SoldTickets = soldTickets,
        //        RemainingSeats = remainingSeats
        //    };

        //    return View(model);
        //}



        //[Authorize]
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> BuyTicket(int id, int quantity = 1)
        //    {
        //        if (quantity < 1) quantity = 1;
        //        if (quantity > 10) quantity = 10; // лимит за сигурност (може да го махнеш)

        //        var ev = await _context.Events
        //            .Include(e => e.Category)
        //            .FirstOrDefaultAsync(e => e.Id == id);

        //        if (ev == null)
        //            return NotFound();

        //    if (ev.Date.Date < DateTime.Today)
        //    {
        //        TempData["Message"] = "Това събитие е минало и не може да се закупи билет.";
        //        return RedirectToAction("Details", new { id });
        //    }


        //    // Колко билета вече са купени за това събитие
        //    var sold = await _context.Tickets.CountAsync(t => t.EventId == id);

        //        // Свободни места
        //        var remaining = ev.Capacity - sold;

        //        if (remaining <= 0)
        //        {
        //            TempData["Message"] = "Събитието е разпродадено. Няма свободни места.";
        //            return RedirectToAction("Details", new { id });
        //        }

        //        if (quantity > remaining)
        //        {
        //            TempData["Message"] = $"Няма достатъчно свободни места. Остават: {remaining}.";
        //            return RedirectToAction("Details", new { id });
        //        }

        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //        // Добавяме quantity на брой билети (редове)
        //        for (int i = 0; i < quantity; i++)
        //        {
        //            _context.Tickets.Add(new Ticket
        //            {
        //                EventId = id,
        //                UserId = userId,
        //                PurchaseDate = DateTime.Now
        //            });
        //        }

        //        await _context.SaveChangesAsync();

        //        TempData["Message"] = $"Успешно закупихте {quantity} билет(а).";
        //        return RedirectToAction("Details", new { id });
        //    }
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EventsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? category, string? city, DateTime? date)
        {
            var query = _context.Events
                .Include(e => e.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                var cat = category.Trim();
                query = query.Where(e => e.Category != null && e.Category.Name == cat);
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                var c = city.Trim();
                query = query.Where(e => e.City.Contains(c));
            }

            if (date.HasValue)
            {
                var d = date.Value.Date;
                query = query.Where(e => e.Date.Date == d);
            }

            var all = await query.OrderBy(e => e.Date).ToListAsync();
            var today = DateTime.Today;

            var vm = new EventsIndexViewModel
            {
                Upcoming = all.Where(e => e.Date.Date >= today).ToList(),
                Past = all.Where(e => e.Date.Date < today).ToList()
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return NotFound();

            // ✅ ВАЖНО: при теб Ticket има Quantity => Count() е грешно.
            // Трябва Sum(Quantity)
            var soldTickets = await _context.Tickets
                .Where(t => t.EventId == id)
                .SumAsync(t => (int?)t.Quantity) ?? 0;

            var remainingSeats = ev.Capacity - soldTickets;

            // Reviews: само одобрени
            var reviewsQuery = _context.Reviews
                .Include(r => r.User)
                .Where(r => r.EventId == id && r.IsApproved);

            var reviewsCount = await reviewsQuery.CountAsync();
            var averageRating = reviewsCount == 0
                ? 0
                : await reviewsQuery.AverageAsync(r => (double)r.Rating);

            var reviews = await reviewsQuery
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            // Права: може да пише review само ако има ПЛАТЕН билет
            var userId = _userManager.GetUserId(User);
            bool canReview = false;
            bool userHasReviewed = false;

            if (!string.IsNullOrEmpty(userId))
            {
                canReview = await _context.Tickets.AnyAsync(t =>
                    t.UserId == userId &&
                    t.EventId == id &&
                    t.IsPaid);

                // 1 review per user per event (проверка)
                userHasReviewed = await _context.Reviews
                    .IgnoreQueryFilters()
                    .AnyAsync(r => r.UserId == userId && r.EventId == id && !r.IsDeleted);
            }

            var model = new EventDetailsViewModel
            {
                Event = ev,
                SoldTickets = soldTickets,
                RemainingSeats = remainingSeats,

                Reviews = reviews,
                ReviewsCount = reviewsCount,
                AverageRating = averageRating,

                CanReview = canReview,
                UserHasReviewed = userHasReviewed
            };

            return View(model);
        }

        // BuyTicket може да остане, но не ти трябва ако купуваш през Cart.
        // Ако го ползваш – виж Стъпка 5 по-долу за поправка.
    }
}
    

