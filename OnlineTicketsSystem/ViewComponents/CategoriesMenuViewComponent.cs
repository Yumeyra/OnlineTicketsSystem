using Microsoft.AspNetCore.Mvc;
using OnlineTicketsSystem.Data;
using Microsoft.EntityFrameworkCore;


namespace OnlineTicketsSystem.ViewComponents
{
    public class CategoriesMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoriesMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }
    }

}

