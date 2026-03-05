
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using OnlineTicketsSystem.Data;

//namespace OnlineTicketsSystem.ViewComponents
//{
//    public class CitiesMenuViewComponent :ViewComponent
//    {
//        private readonly ApplicationDbContext _context;

//        public CitiesMenuViewComponent(ApplicationDbContext context)
//        {
//            _context = context;
//        }




//        public async Task<IViewComponentResult> InvokeAsync()
//        {
//            var cities = await _context.Cities
//                .OrderBy(c => c.Name)
//                .ToListAsync();

//            return View(cities);
//        }

//    }
//}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Data;
using OnlineTicketsSystem.ViewModels;

namespace OnlineTicketsSystem.ViewComponents
{
    public class CitiesMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CitiesMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cities = await _context.Cities
                .OrderBy(c => c.Region)
                .ThenBy(c => c.Name)
                .Select(c => new { c.Name, c.Slug, c.Region })
                .ToListAsync();

            var vm = new CitiesMenuVm();

            vm.Regions = cities
                .Select(c => c.Region)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            foreach (var region in vm.Regions)
            {
                vm.CitiesByRegion[region] = cities
                    .Where(c => c.Region == region)
                    .Select(c => new CityMenuItemVm
                    {
                        Name = c.Name,
                        Slug = c.Slug
                    })
                    .ToList();
            }

            return View(vm);
        }
    }
}