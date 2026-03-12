//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using OnlineTicketsSystem.Data;
//using OnlineTicketsSystem.Models;
//using OnlineTicketsSystem.ViewModels;
//using System.Security.Claims;


//namespace OnlineTicketsSystem.Controllers
//{
//    public class EventsController : Controller
//    {

//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<IdentityUser> _userManager;

//        public EventsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
//        {
//            _context = context;
//            _userManager = userManager;
//        }

//        public async Task<IActionResult> Index(string? category, string? city, DateTime? date)
//        {
//            var query = _context.Events
//                .Include(e => e.Category)
//                .AsQueryable();

//            if (!string.IsNullOrWhiteSpace(category))
//            {
//                var cat = category.Trim();
//                query = query.Where(e => e.Category != null && e.Category.Name == cat);
//            }

//            if (!string.IsNullOrWhiteSpace(city))
//            {
//                var c = city.Trim();
//                query = query.Where(e => e.City.Contains(c));
//            }

//            if (date.HasValue)
//            {
//                var d = date.Value.Date;
//                query = query.Where(e => e.Date.Date == d);
//            }

//            var all = await query.OrderBy(e => e.Date).ToListAsync();
//            var today = DateTime.Today;

//            var vm = new EventsIndexViewModel
//            {
//                Upcoming = all.Where(e => e.Date.Date >= today).ToList(),
//                Past = all.Where(e => e.Date.Date < today).ToList()
//            };

//            return View(vm);
//        }

//        public async Task<IActionResult> Details(int id)
//        {
//            var ev = await _context.Events
//                .Include(e => e.Category)
//                .FirstOrDefaultAsync(e => e.Id == id);

//            if (ev == null) return NotFound();

//            // ✅ ВАЖНО: при теб Ticket има Quantity => Count() е грешно.
//            // Трябва Sum(Quantity)
//            var soldTickets = await _context.Tickets
//                .Where(t => t.EventId == id)
//                .SumAsync(t => (int?)t.Quantity) ?? 0;

//            var remainingSeats = ev.Capacity - soldTickets;

//            // Reviews: само одобрени
//            var reviewsQuery = _context.Reviews
//                .Include(r => r.User)
//                .Where(r => r.EventId == id && r.IsApproved);

//            var reviewsCount = await reviewsQuery.CountAsync();
//            var averageRating = reviewsCount == 0
//                ? 0
//                : await reviewsQuery.AverageAsync(r => (double)r.Rating);

//            var reviews = await reviewsQuery
//                .OrderByDescending(r => r.CreatedAt)
//                .ToListAsync();

//            // Права: може да пише review само ако има ПЛАТЕН билет
//            var userId = _userManager.GetUserId(User);
//            bool canReview = false;
//            bool userHasReviewed = false;

//            if (!string.IsNullOrEmpty(userId))
//            {
//                canReview = await _context.Tickets.AnyAsync(t =>
//                    t.UserId == userId &&
//                    t.EventId == id &&
//                    t.IsPaid);

//                // 1 review per user per event (проверка)
//                userHasReviewed = await _context.Reviews
//                    .IgnoreQueryFilters()
//                    .AnyAsync(r => r.UserId == userId && r.EventId == id && !r.IsDeleted);
//            }

//            var model = new EventDetailsViewModel
//            {
//                Event = ev,
//                SoldTickets = soldTickets,
//                RemainingSeats = remainingSeats,

//                Reviews = reviews,
//                ReviewsCount = reviewsCount,
//                AverageRating = averageRating,

//                CanReview = canReview,
//                UserHasReviewed = userHasReviewed
//            };

//            return View(model);
//        }

//        // BuyTicket може да остане, но не ти трябва ако купуваш през Cart.
//        // Ако го ползваш – виж Стъпка 5 по-долу за поправка.
//    }
//}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Data;
using OnlineTicketsSystem.ViewModels;

namespace OnlineTicketsSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EventsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //public async Task<IActionResult> Index(
        //    string? searchTerm,
        //    string? category,
        //    string? city,
        //    DateTime? date,
        //    int page = 1)
        //{
        //    const int pageSize = 6;

        //    var query = _context.Events
        //        .Include(e => e.Category)
        //        .AsQueryable();

        //    if (!string.IsNullOrWhiteSpace(searchTerm))
        //    {
        //        var term = searchTerm.Trim();
        //        query = query.Where(e =>
        //            e.Title.Contains(term) ||
        //            e.City.Contains(term) ||
        //            e.Venue.Contains(term));
        //    }

        //    if (!string.IsNullOrWhiteSpace(category))
        //    {
        //        var cat = category.Trim();
        //        query = query.Where(e => e.Category != null && e.Category.Name == cat);
        //    }

        //    if (!string.IsNullOrWhiteSpace(city))
        //    {
        //        var c = city.Trim();
        //        query = query.Where(e => e.City == c);
        //    }

        //    if (date.HasValue)
        //    {
        //        var d = date.Value.Date;
        //        query = query.Where(e => e.Date.Date == d);
        //    }

        //    var today = DateTime.Today;

        //    var cities = await _context.Events
        //        .Select(e => e.City)
        //        .Where(c => !string.IsNullOrEmpty(c))
        //        .Distinct()
        //        .OrderBy(c => c)
        //        .ToListAsync();

        //    var categories = await _context.Categories
        //        .Select(c => c.Name)
        //        .Where(n => !string.IsNullOrEmpty(n))
        //        .Distinct()
        //        .OrderBy(n => n)
        //        .ToListAsync();

        //    var upcomingQuery = query
        //        .Where(e => e.Date.Date >= today)
        //        .OrderBy(e => e.Date);

        //    var pastQuery = query
        //        .Where(e => e.Date.Date < today)
        //        .OrderByDescending(e => e.Date);

        //    var totalUpcoming = await upcomingQuery.CountAsync();
        //    var totalPages = (int)Math.Ceiling(totalUpcoming / (double)pageSize);

        //    if (totalPages == 0)
        //        totalPages = 1;

        //    if (page < 1)
        //        page = 1;

        //    if (page > totalPages)
        //        page = totalPages;

        //    var upcoming = await upcomingQuery
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    var past = await pastQuery
        //        .Take(6)
        //        .ToListAsync();

        //    var vm = new EventsIndexViewModel
        //    {
        //        Upcoming = upcoming,
        //        Past = past,

        //        SearchTerm = searchTerm,
        //        SelectedCity = city,
        //        SelectedCategory = category,
        //        SelectedDate = date,

        //        Cities = cities,
        //        Categories = categories,

        //        CurrentPage = page,
        //        TotalPages = totalPages
        //    };
        //    ViewBag.ShowFilters = string.IsNullOrEmpty(category);
        //    return View(vm);
        //}
        public async Task<IActionResult> Index(
    string? searchTerm,
    string? category,
    string? city,
    DateTime? date,
    int page = 1)
        {
            const int pageSize = 6;

            var query = _context.Events
                .Include(e => e.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(e =>
                    e.Title.Contains(term) ||
                    e.City.Contains(term) ||
                    e.Venue.Contains(term));
            }

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

            var today = DateTime.Today;

            var cities = await _context.Events
                .Select(e => e.City)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var categories = await _context.Categories
                .Select(c => c.Name)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync();

            var upcomingQuery = query
                .Where(e => e.Date.Date >= today)
                .OrderBy(e => e.Date);

            var pastQuery = query
                .Where(e => e.Date.Date < today)
                .OrderByDescending(e => e.Date);

            var totalUpcoming = await upcomingQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalUpcoming / (double)pageSize);

            if (totalPages == 0)
                totalPages = 1;

            if (page < 1)
                page = 1;

            if (page > totalPages)
                page = totalPages;

            var upcoming = await upcomingQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var past = await pastQuery
                .Take(6)
                .ToListAsync();

            var vm = new EventsIndexViewModel
            {
                Upcoming = upcoming,
                Past = past,

                SearchTerm = searchTerm,
                SelectedCity = city,
                SelectedCategory = category,
                SelectedDate = date,

                Cities = cities,
                Categories = categories,

                CurrentPage = page,
                TotalPages = totalPages
            };

            ViewBag.ShowFilters = string.IsNullOrWhiteSpace(category);

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return NotFound();

            var soldTickets = await _context.Tickets
                .Where(t => t.EventId == id)
                .SumAsync(t => (int?)t.Quantity) ?? 0;

            var remainingSeats = ev.Capacity - soldTickets;

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

            var userId = _userManager.GetUserId(User);
            bool canReview = false;
            bool userHasReviewed = false;

            if (!string.IsNullOrEmpty(userId))
            {
                canReview = await _context.Tickets.AnyAsync(t =>
                    t.UserId == userId &&
                    t.EventId == id &&
                    t.IsPaid);

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
    }
}
