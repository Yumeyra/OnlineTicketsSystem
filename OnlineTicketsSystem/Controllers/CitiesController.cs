//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using OnlineTicketsSystem.Data;
//using OnlineTicketsSystem.Helpers;
//using OnlineTicketsSystem.Services.Interfaces;

//namespace OnlineTicketsSystem.Controllers
//{

//    [Route("Cities")]
//    public class CitiesController : Controller
//    {

//        private readonly ICityService _cityService;
//        public CitiesController(ICityService cityService)
//        {
//            _cityService = cityService;
//        }

//        // /Cities -> всички градове от таблица Cities
//        [HttpGet("")]
//        public async Task<IActionResult> Index()
//        {
//            var cities = await _cityService.GetAllCitiesAsync();
//            return View(cities);
//        }

//        // /Cities/{slug} -> събития за този град
//        [HttpGet("{slug}")]
//        public async Task<IActionResult> ByCity(string slug)
//        {
//            var city = await _cityService.GetCityBySlugAsync(slug);
//            if (city == null) return NotFound();

//            var eventsInCity = await _cityService.GetEventsByCityNameAsync(city.Name);

//            ViewData["CityName"] = city.Name;
//            return View(eventsInCity);
//        }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using OnlineTicketsSystem.Services.Interfaces;

namespace OnlineTicketsSystem.Controllers
{
    [Route("Cities")]
    public class CitiesController : Controller
    {
        private readonly ICityService _cityService;

        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        // /Cities -> всички градове
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var cities = await _cityService.GetAllCitiesAsync();

            if (!cities.Any())
                TempData["Info"] = "Няма налични градове.";

            return View(cities);
        }

        // /Cities/{slug} -> събития за този град
        [HttpGet("{slug}")]
        public async Task<IActionResult> ByCity(string slug)
        {
            var city = await _cityService.GetCityBySlugAsync(slug);

            if (city == null)
            {
                TempData["Error"] = "Градът не беше намерен.";
                return RedirectToAction("Index");
            }

            var eventsInCity = await _cityService.GetEventsByCityNameAsync(city.Name);

            if (!eventsInCity.Any())
                TempData["Info"] = $"Няма събития в {city.Name}.";

            ViewData["CityName"] = city.Name;
            return View(eventsInCity);
        }
    }
}


