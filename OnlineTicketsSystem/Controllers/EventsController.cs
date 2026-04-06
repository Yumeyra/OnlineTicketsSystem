

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineTicketsSystem.Services.Interfaces;

namespace OnlineTicketsSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly UserManager<IdentityUser> _userManager;

        public EventsController(
            IEventService eventService,
            UserManager<IdentityUser> userManager)
        {
            _eventService = eventService;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(
            string? searchTerm,
            string? category,
            string? city,
            DateTime? date,
            int page = 1)
        {
            var vm = await _eventService.GetEventsIndexAsync(
                searchTerm,
                category,
                city,
                date,
                page);

            ViewBag.ShowFilters = string.IsNullOrWhiteSpace(category);

            return View(vm);
        }


        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);

            var model = await _eventService.GetEventDetailsAsync(id, userId);

            if (model == null)
                return NotFound();

            return View(model);
        }
    }
}

//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using OnlineTicketsSystem.Services.Interfaces;

//namespace OnlineTicketsSystem.Controllers
//{
//    public class EventsController : Controller
//    {
//        private readonly IEventService _eventService;
//        private readonly UserManager<IdentityUser> _userManager;

//        public EventsController(
//            IEventService eventService,
//            UserManager<IdentityUser> userManager)
//        {
//            _eventService = eventService;
//            _userManager = userManager;
//        }

//        public async Task<IActionResult> Index(
//            string? searchTerm,
//            string? category,
//            string? city,
//            DateTime? date,
//            int page = 1)
//        {
//            var vm = await _eventService.GetEventsIndexAsync(
//                searchTerm,
//                category,
//                city,
//                date,
//                page);

//            // Ако няма резултати → toast
//            if (!vm.Event.Any())
//            {
//                TempData["Info"] = "Няма намерени събития по зададените критерии.";
//            }

//            ViewBag.ShowFilters = string.IsNullOrWhiteSpace(category);

//            return View(vm);
//        }

//        public async Task<IActionResult> Details(int id)
//        {
//            var userId = _userManager.GetUserId(User);

//            var model = await _eventService.GetEventDetailsAsync(id, userId);

//            if (model == null)
//            {
//                TempData["Error"] = "Събитието не беше намерено.";
//                return RedirectToAction("Index");
//            }

//            return View(model);
//        }
//    }
//}

