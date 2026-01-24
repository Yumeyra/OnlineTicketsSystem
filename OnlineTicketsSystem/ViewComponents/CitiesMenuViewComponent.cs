
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
            var cities = await _context.Cities
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(cities);
        }

    }
}

