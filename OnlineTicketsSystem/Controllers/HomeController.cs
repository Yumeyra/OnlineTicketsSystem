using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Models;
using OnlineTicketsSystem.ViewModels;
using System.Diagnostics;
using OnlineTicketsSystem.Data;


namespace OnlineTicketsSystem.Controllers
{
    public class HomeController : Controller
    {
       // private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }



        //public async Task<IActionResult> Index(int? categoryId, string? city, string? dateRange, string? priceRange)
        //{
        //    var today = DateTime.Today;

        //    var query = _context.Events
        //        .Include(e => e.Category)
        //        .Where(e => e.Date >= today)
        //        .AsQueryable();

        //    // category
        //    if (categoryId.HasValue)
        //        query = query.Where(e => e.CategoryId == categoryId.Value);

        //    // city (текстово търсене)
        //    if (!string.IsNullOrWhiteSpace(city))
        //        query = query.Where(e => e.City.Contains(city));

        //    // date range
        //    if (dateRange == "week")
        //        query = query.Where(e => e.Date <= today.AddDays(7));
        //    else if (dateRange == "month")
        //        query = query.Where(e => e.Date <= today.AddMonths(1));

        //    // price range
        //    if (!string.IsNullOrWhiteSpace(priceRange))
        //    {
        //        if (priceRange == "0-10")
        //            query = query.Where(e => e.Price >= 0 && e.Price <= 10);
        //        else if (priceRange == "10-20")
        //            query = query.Where(e => e.Price > 10 && e.Price <= 20);
        //        else if (priceRange == "20+")
        //            query = query.Where(e => e.Price >= 20);
        //    }

        //    var upcoming = await query
        //        .OrderBy(e => e.Date)
        //        .Take(12)
        //        .ToListAsync();

        //    var categories = await _context.Categories
        //        .OrderBy(c => c.Name)
        //        .ToListAsync();

        //    var cities = await _context.Events
        //        .Select(e => e.City)
        //        .Distinct()
        //        .OrderBy(c => c)
        //        .Take(8)
        //        .ToListAsync();

        //    var vm = new HomeIndexVm
        //    {
        //        UpcomingEvents = upcoming,
        //        Categories = categories,
        //        Cities = cities,

        //        SelectedCategoryId = categoryId,
        //        City = city,
        //        DateRange = dateRange,
        //        PriceRange = priceRange
        //    };

        //    return View(vm);
        //}
        public async Task<IActionResult> Index(int? categoryId,string? selectedRegion,string? selectedCity,string? dateRange,string? priceRange)
        {
            List<string> citiesInRegion = new();

            if (!string.IsNullOrWhiteSpace(selectedRegion))
            {
                citiesInRegion = await _context.Cities
                    .Where(c => c.Region == selectedRegion && !c.IsDeleted)
                    .Select(c => c.Name)
                    .Distinct()
                    .OrderBy(n => n)
                    .ToListAsync();
            }
            var today = DateTime.Today;

            var query = _context.Events
                .Include(e => e.Category)
                .Where(e => e.Date >= today)
                .AsQueryable();

            // category
            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryId == categoryId.Value);

            // ✅ city (точно съвпадение е по-професионално от Contains)
            if (!string.IsNullOrWhiteSpace(selectedCity))
                query = query.Where(e => e.City == selectedCity);

            // date range
            if (dateRange == "week")
                query = query.Where(e => e.Date <= today.AddDays(7));
            else if (dateRange == "month")
                query = query.Where(e => e.Date <= today.AddMonths(1));

            // price range
            if (!string.IsNullOrWhiteSpace(priceRange))
            {
                if (priceRange == "0-10")
                    query = query.Where(e => e.Price >= 0 && e.Price <= 10);
                else if (priceRange == "10-20")
                    query = query.Where(e => e.Price > 10 && e.Price <= 20);
                else if (priceRange == "20+")
                    query = query.Where(e => e.Price >= 20);
            }

            var upcoming = await query
                .OrderBy(e => e.Date)
                .Take(12)
                .ToListAsync();

            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            // ✅ Regions от Cities таблицата
            var regions = await _context.Cities
                .Select(c => c.Region)
                .Distinct()
                .OrderBy(r => r)
                .ToListAsync();

            // ✅ Cities за избраната област
            //List<string> citiesInRegion = new();
            if (!string.IsNullOrWhiteSpace(selectedRegion))
            {
                citiesInRegion = await _context.Cities
                    .Where(c => c.Region == selectedRegion)
                    .OrderBy(c => c.Name)
                    .Select(c => c.Name)
                    .ToListAsync();
            }

            // (по желание) за секцията "Популярни градове" може да си остане както е
            var popularCities = await _context.Events
                .Select(e => e.City)
                .Distinct()
                .OrderBy(c => c)
                .Take(8)
                .ToListAsync();

            var vm = new HomeIndexVm
            {
                UpcomingEvents = upcoming,
                Categories = categories,
                Cities = popularCities,

                SelectedCategoryId = categoryId,
                SelectedRegion = selectedRegion,
                SelectedCity = selectedCity,
                DateRange = dateRange,
                PriceRange = priceRange,

                Regions = regions,
                CitiesInRegion = citiesInRegion
            };

            return View(vm);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
