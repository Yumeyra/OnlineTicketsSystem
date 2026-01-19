using Microsoft.AspNetCore.Mvc;
using OnlineTicketsSystem.Data;
using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Models;

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
            var events = _context.Events.Include(e => e.Category).AsQueryable();

            if (!string.IsNullOrEmpty(category))
                events = events.Where(e => e.Category.Name == category);

            if (!string.IsNullOrEmpty(city))
                events = events.Where(e => e.City.Contains(city));

            if (date.HasValue)
                events = events.Where(e => e.Date.Date == date.Value.Date);

            return View(await events.ToListAsync());
        }

        // GET: /Events/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null)
                return NotFound();

            return View(ev);
        }
    }
}

