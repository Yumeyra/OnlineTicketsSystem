using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Data;
using OnlineTicketsSystem.Models;
using OnlineTicketsSystem.Services.Interfaces;
using OnlineTicketsSystem.ViewModels;
namespace OnlineTicketsSystem.Services
{
    public class AdminEventService : IAdminEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminEventService(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await _context.Events
                .Include(e => e.Category)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<SelectList> GetCategoriesSelectListAsync(int? selectedId = null)
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            return new SelectList(categories, "Id", "Name", selectedId);
        }

        public async Task CreateEventAsync(Event ev, IFormFile? posterFile)
        {
            if (posterFile != null && posterFile.Length > 0)
            {
                ev.ImageUrl = await SavePosterAsync(posterFile, ev.Title);
            }

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            return await _context.Events.FindAsync(id);
        }

        public async Task UpdateEventAsync(int id, Event ev, IFormFile? posterFile)
        {
            var dbEv = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (dbEv == null) return;

            dbEv.Title = ev.Title;
            dbEv.Description = ev.Description;
            dbEv.City = ev.City;
            dbEv.Venue = ev.Venue;
            dbEv.Date = ev.Date;
            dbEv.Capacity = ev.Capacity;
            dbEv.Price = ev.Price;
            dbEv.CategoryId = ev.CategoryId;

            if (posterFile != null && posterFile.Length > 0)
            {
                DeleteLocalFileIfExists(dbEv.ImageUrl);
                dbEv.ImageUrl = await SavePosterAsync(posterFile, dbEv.Title);
            }

            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteEventAsync(int id)
        {
            var ev = await _context.Events
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return;

            ev.IsDeleted = true;
            ev.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<AdminDashboardViewModel> GetDashboardDataAsync()
        {
            var now = DateTime.Now;

            var totalEvents = await _context.Events.CountAsync();

            var paidTickets = _context.Tickets.Where(t => t.IsPaid);

            var totalSoldTickets = await paidTickets.SumAsync(t => (int?)t.Quantity) ?? 0;
            var totalRevenue = await paidTickets.SumAsync(t => (decimal?)(t.UnitPrice * t.Quantity)) ?? 0;

            var upcomingEvents = await _context.Events.CountAsync(e => e.Date >= now);
            var pastEvents = await _context.Events.CountAsync(e => e.Date < now);

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
                        .Sum(t => (decimal?)(t.UnitPrice * t.Quantity)) ?? 0
                })
                .ToListAsync();

            foreach (var e in eventStats)
            {
                e.RemainingSeats = Math.Max(0, e.Capacity - e.SoldTickets);
            }

            var topEvents = eventStats
                .OrderByDescending(e => e.SoldTickets)
                .Take(5)
                .ToList();

            return new AdminDashboardViewModel
            {
                TotalEvents = totalEvents,
                TotalSoldTickets = totalSoldTickets,
                TotalRevenue = totalRevenue,
                UpcomingEvents = upcomingEvents,
                PastEvents = pastEvents,
                TopEvents = topEvents
            };
        }

        // 🔧 Helpers
        private async Task<string> SavePosterAsync(IFormFile file, string title)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "events");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsDir, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/events/{fileName}";
        }

        private void DeleteLocalFileIfExists(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return;

            var full = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/'));
            if (File.Exists(full)) File.Delete(full);
        }
    }
}

