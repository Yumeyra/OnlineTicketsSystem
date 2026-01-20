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

        // /Cities  -> страница с всички градове
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var cities = await _context.Events
                .Select(e => e.City)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return View(cities);
        }

        // /Cities/Burgas -> събития в Бургас
        [HttpGet("{slug}")]
        public async Task<IActionResult> ByCity(string slug)
        {
            if (!CitySlugMap.TryGetBgName(slug, out var cityBg))
                return NotFound();

            var eventsInCity = await _context.Events
                .Include(e => e.Category)
                .Where(e => e.City == cityBg)
                .OrderBy(e => e.Date)
                .ToListAsync();

            ViewData["CityName"] = cityBg;
            return View(eventsInCity);
        }
    }
}

