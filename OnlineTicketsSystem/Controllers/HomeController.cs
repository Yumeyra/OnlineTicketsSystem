
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