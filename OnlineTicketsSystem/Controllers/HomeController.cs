
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using OnlineTicketsSystem.Models;
//using OnlineTicketsSystem.ViewModels;
//using System.Diagnostics;
//using OnlineTicketsSystem.Data;

//namespace OnlineTicketsSystem.Controllers
//{
//    public class HomeController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public HomeController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<IActionResult> Index(
//            int? categoryId,
//            string? selectedRegion,
//            string? selectedCity,
//            string? dateRange,
//            string? priceRange)
//        {
//            List<string> citiesInRegion = new();
//            var today = DateTime.Today;

//            if (!string.IsNullOrWhiteSpace(selectedRegion))
//            {
//                citiesInRegion = await _context.Cities
//                    .Where(c => c.Region == selectedRegion && !c.IsDeleted)
//                    .Select(c => c.Name)
//                    .Distinct()
//                    .OrderBy(n => n)
//                    .ToListAsync();
//            }

//            var query = _context.Events
//                .Include(e => e.Category)
//                .Where(e => e.Date >= today)
//                .AsQueryable();

//            if (categoryId.HasValue)
//                query = query.Where(e => e.CategoryId == categoryId.Value);

//            if (!string.IsNullOrWhiteSpace(selectedCity))
//                query = query.Where(e => e.City == selectedCity);

//            if (dateRange == "week")
//                query = query.Where(e => e.Date <= today.AddDays(7));
//            else if (dateRange == "month")
//                query = query.Where(e => e.Date <= today.AddMonths(1));

//            if (!string.IsNullOrWhiteSpace(priceRange))
//            {
//                if (priceRange == "0-10")
//                    query = query.Where(e => e.Price >= 0 && e.Price <= 10);
//                else if (priceRange == "10-20")
//                    query = query.Where(e => e.Price > 10 && e.Price <= 20);
//                else if (priceRange == "20+")
//                    query = query.Where(e => e.Price >= 20);
//            }

//            var upcoming = await query
//                .OrderBy(e => e.Date)
//                .Take(12)
//                .ToListAsync();

//            var categories = await _context.Categories
//                .OrderBy(c => c.Name)
//                .ToListAsync();

//            var regions = await _context.Cities
//                .Select(c => c.Region)
//                .Distinct()
//                .OrderBy(r => r)
//                .ToListAsync();

//            if (!string.IsNullOrWhiteSpace(selectedRegion))
//            {
//                citiesInRegion = await _context.Cities
//                    .Where(c => c.Region == selectedRegion)
//                    .OrderBy(c => c.Name)
//                    .Select(c => c.Name)
//                    .ToListAsync();
//            }


//            var preferredCityOrder = new List<string>
//{
//    "София",
//    "Пловдив",
//    "Варна",
//    "Бургас",
//    "Русе",
//    "Стара Загора",
//    "Плевен",
//    "Велико Търново"
//};

//            var popularCities = await _context.Cities
//                .Where(c => !c.IsDeleted && preferredCityOrder.Contains(c.Name))
//                .ToListAsync();

//            popularCities = popularCities
//                .OrderBy(c => preferredCityOrder.IndexOf(c.Name))
//                .ToList();

//            var heroImages = await _context.Events
//                .Where(e => !string.IsNullOrWhiteSpace(e.ImageUrl))
//                .Select(e => new
//                {
//                    e.ImageUrl,
//                    SoldTickets = _context.Tickets
//                        .Where(t => t.EventId == e.Id && t.IsPaid)
//                        .Sum(t => (int?)t.Quantity) ?? 0
//                })
//                .OrderByDescending(x => x.SoldTickets)
//                .ThenBy(x => x.ImageUrl)
//                .Take(5)
//                .Select(x => x.ImageUrl!)
//                .ToListAsync();

//            if (!heroImages.Any())
//            {
//                heroImages = await _context.Events
//                    .Where(e => !string.IsNullOrWhiteSpace(e.ImageUrl))
//                    .OrderByDescending(e => e.Date)
//                    .Take(5)
//                    .Select(e => e.ImageUrl!)
//                    .ToListAsync();
//            }

//            ViewBag.HeroImages = heroImages;

//            var vm = new HomeIndexVm
//            {
//                UpcomingEvents = upcoming,
//                Categories = categories,
//                Cities = popularCities,

//                SelectedCategoryId = categoryId,
//                SelectedRegion = selectedRegion,
//                SelectedCity = selectedCity,
//                DateRange = dateRange,
//                PriceRange = priceRange,

//                Regions = regions,
//                CitiesInRegion = citiesInRegion
//            };

//            return View(vm);
//        }

//        public IActionResult Privacy()
//        {
//            return View();
//        }

//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }
//    }
//}
using Microsoft.AspNetCore.Mvc;
using OnlineTicketsSystem.Models;
using OnlineTicketsSystem.Services.Interfaces;
using OnlineTicketsSystem.ViewModels;
using System.Diagnostics;

namespace OnlineTicketsSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index(
            int? categoryId,
            string? selectedRegion,
            string? selectedCity,
            string? dateRange,
            string? priceRange)
        {
            var vm = await _homeService.GetHomeDataAsync(
                categoryId,
                selectedRegion,
                selectedCity,
                dateRange,
                priceRange);

            //ViewBag.HeroImages = vm.HeroImages;

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}