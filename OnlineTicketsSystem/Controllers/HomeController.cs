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


        //public IActionResult Index()
        //{
        //    return View();
        //}
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            var upcoming = await _context.Events
                .Include(e => e.Category)
                .Where(e => e.Date >= today)
                .OrderBy(e => e.Date)
                .Take(8)
                .ToListAsync();

            var cities = await _context.Events
                .Select(e => e.City)
                .Distinct()
                .OrderBy(c => c)
                .Take(8)
                .ToListAsync();

            var categories = await _context.Categories
    .OrderBy(c => c.Name)
    .Select(c => c.Name)
    .ToListAsync();


            var vm = new HomeIndexVm
            {
                UpcomingEvents = upcoming,
                Cities = cities,
                 Categories = categories
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
