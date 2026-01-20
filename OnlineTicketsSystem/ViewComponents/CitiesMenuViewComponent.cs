
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Data;

namespace OnlineTicketsSystem.ViewComponents
{
    public class CitiesMenuViewComponent :ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CitiesMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cities = await _context.Events
                .Select(e => e.City)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return View(cities);
        }
    }
}

