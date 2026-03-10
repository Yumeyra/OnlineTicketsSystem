using Microsoft.AspNetCore.Identity;
using OnlineTicketsSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineTicketsSystem.Data;
using OnlineTicketsSystem.Models;

namespace OnlineTicketsSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminEventsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        //public AdminEventsController(ApplicationDbContext context, IWebHostEnvironment env)
        //{
        //    _context = context;
        //    _env = env;
        //}
        public AdminEventsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /AdminEvents
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events
                .Include(e => e.Category)
                .OrderBy(e => e.Date)
                .ToListAsync();

            return View(events);
        }

        // GET: /AdminEvents/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(
                await _context.Categories.OrderBy(c => c.Name).ToListAsync(),
                "Id", "Name"
            );

            return View();
        }

        // POST: /AdminEvents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event ev, IFormFile? posterFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(
                    await _context.Categories.OrderBy(c => c.Name).ToListAsync(),
                    "Id", "Name", ev.CategoryId
                );
                return View(ev);
            }

            // upload poster
            if (posterFile != null && posterFile.Length > 0)
            {
                ev.ImageUrl = await SavePosterAsync(posterFile, ev.Title);
            }

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminEvents/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            ViewBag.Categories = new SelectList(
                await _context.Categories.OrderBy(c => c.Name).ToListAsync(),
                "Id", "Name", ev.CategoryId
            );

            return View(ev);
        }

        // POST: /AdminEvents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event ev, IFormFile? posterFile)
        {
            if (id != ev.Id) return NotFound();

            var dbEv = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (dbEv == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(
                    await _context.Categories.OrderBy(c => c.Name).ToListAsync(),
                    "Id", "Name", ev.CategoryId
                );
                return View(ev);
            }

            // copy editable fields
            dbEv.Title = ev.Title;
            dbEv.Description = ev.Description;
            dbEv.City = ev.City;
            dbEv.Venue = ev.Venue;
            dbEv.Date = ev.Date;
            dbEv.Capacity = ev.Capacity;
            dbEv.Price = ev.Price;
            dbEv.CategoryId = ev.CategoryId;

            // upload new poster if provided
            if (posterFile != null && posterFile.Length > 0)
            {
                // optional: delete old file if it's local
                DeleteLocalFileIfExists(dbEv.ImageUrl);

                dbEv.ImageUrl = await SavePosterAsync(posterFile, dbEv.Title);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminEvents/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ev = await _context.Events
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return NotFound();

            ev.IsDeleted = true;
            ev.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SavePosterAsync(IFormFile file, string title)
        {
            // allow only images
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
                throw new InvalidOperationException("Позволени са само JPG, PNG, WEBP изображения.");

            // max 5MB
            if (file.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException("Файлът е твърде голям (макс 5MB).");

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "events");
            Directory.CreateDirectory(uploadsDir);

            var safeTitle = string.Concat(title.Where(ch => char.IsLetterOrDigit(ch) || ch == '-' || ch == '_' || ch == ' '))
                .Trim()
                .Replace(' ', '_');

            var fileName = $"evt_{DateTime.UtcNow:yyyyMMdd_HHmmss}_{safeTitle}_{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsDir, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            // DB path
            return $"/uploads/events/{fileName}";
        }

        private void DeleteLocalFileIfExists(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return;
            if (!imageUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase)) return;

            var relative = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var full = Path.Combine(_env.WebRootPath, relative);

            if (System.IO.File.Exists(full))
                System.IO.File.Delete(full);
        }
        public async Task<IActionResult> Dashboard()
        {
            var now = DateTime.Now;

            var totalEvents = await _context.Events.CountAsync();

            var paidTicketsQuery = _context.Tickets
                .Where(t => t.IsPaid);

            var totalSoldTickets = await paidTicketsQuery
                .SumAsync(t => (int?)t.Quantity) ?? 0;

            var totalRevenue = await paidTicketsQuery
                .SumAsync(t => (decimal?)(t.UnitPrice * t.Quantity)) ?? 0m;

            var upcomingEvents = await _context.Events
                .CountAsync(e => e.Date >= now);

            var pastEvents = await _context.Events
                .CountAsync(e => e.Date < now);

            var eventStats = await _context.Events
                .Select(e => new TopEventViewModel
                {
                    EventId = e.Id,
                    Title = e.Title,
                    Capacity = e.Capacity,
                    SoldTickets = _context.Tickets
                        .Where(t => t.EventId == e.Id && t.IsPaid)
                        .Sum(t => (int?)t.Quantity) ?? 0,
                    Revenue = _context.Tickets
                        .Where(t => t.EventId == e.Id && t.IsPaid)
                        .Sum(t => (decimal?)(t.UnitPrice * t.Quantity)) ?? 0m
                })
                .ToListAsync();

            foreach (var item in eventStats)
            {
                item.RemainingSeats = item.Capacity - item.SoldTickets;
                if (item.RemainingSeats < 0)
                {
                    item.RemainingSeats = 0;
                }
            }

            var soldOutEventList = eventStats
                .Where(e => e.SoldTickets >= e.Capacity)
                .OrderBy(e => e.Title)
                .Select(e => new SoldOutEventViewModel
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    Date = _context.Events.Where(x => x.Id == e.EventId).Select(x => x.Date).FirstOrDefault(),
                    Capacity = e.Capacity,
                    SoldTickets = e.SoldTickets
                })
                .ToList();

            var soldOutEvents = soldOutEventList.Count;

            var topEvents = eventStats
                .OrderByDescending(e => e.SoldTickets)
                .ThenByDescending(e => e.Revenue)
                .Take(5)
                .ToList();

            var recentPurchasesRaw = await _context.Tickets
    .Include(t => t.Event)
    .Where(t => t.IsPaid)
    .OrderByDescending(t => t.PaidAt)
    .Take(10)
    .Select(t => new
    {
        t.Id,
        EventTitle = t.Event.Title,
        t.UserId,
        t.Quantity,
        TotalPrice = t.UnitPrice * t.Quantity,
        t.PaidAt
    })
    .ToListAsync();

            var recentPurchases = new List<RecentPurchaseViewModel>();

            foreach (var item in recentPurchasesRaw)
            {
                var user = await _userManager.FindByIdAsync(item.UserId);

                recentPurchases.Add(new RecentPurchaseViewModel
                {
                    TicketId = item.Id,
                    EventTitle = item.EventTitle,
                    UserId = item.UserId,
                    UserEmail = user?.Email ?? "-",
                    UserName = user?.UserName ?? "-",
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                    PaidAt = item.PaidAt
                });
            }

            var model = new AdminDashboardViewModel
            {
                TotalEvents = totalEvents,
                TotalSoldTickets = totalSoldTickets,
                TotalRevenue = totalRevenue,
                UpcomingEvents = upcomingEvents,
                PastEvents = pastEvents,
                SoldOutEvents = soldOutEvents,
                TopEvents = topEvents,
                RecentPurchases = recentPurchases,
                SoldOutEventList = soldOutEventList
            };

            return View(model);
        }
    }
}