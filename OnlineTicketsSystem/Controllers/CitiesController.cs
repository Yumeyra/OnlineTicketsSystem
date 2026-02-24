using Microsoft.AspNetCore.Mvc;
using OnlineTicketsSystem.Helpers;
using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Data;

namespace OnlineTicketsSystem.Controllers
{
   
    [Route("Cities")]
    public class CitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // /Cities -> всички градове от таблица Cities
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var cities = await _context.Cities
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(cities);
        }

        // /Cities/{slug} -> събития за този град
        [HttpGet("{slug}")]
        public async Task<IActionResult> ByCity(string slug)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Slug == slug);
            if (city == null) return NotFound();

            // ВАЖНО: тук приемаме, че Event.City е текст и трябва да съвпадне с city.Name
            var eventsInCity = await _context.Events
                .Include(e => e.Category)
                .Where(e => e.City == city.Name)
                .OrderBy(e => e.Date)
                .ToListAsync();

            ViewData["CityName"] = city.Name;
            return View(eventsInCity);
        }
    }

}


