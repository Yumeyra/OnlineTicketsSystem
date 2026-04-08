

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineTicketsSystem.Models;
using OnlineTicketsSystem.Services;
using OnlineTicketsSystem.Services.Interfaces;

namespace OnlineTicketsSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminEventsController : Controller
    {
        private readonly IAdminEventService _service;

        public AdminEventsController(IAdminEventService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _service.GetAllEventsAsync();
            return View(events);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _service.GetCategoriesSelectListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Event ev, IFormFile? posterFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = "Моля, попълнете всички полета коректно.";
                ViewBag.Categories = await _service.GetCategoriesSelectListAsync(ev.CategoryId);
                return View(ev);
            }

            try
            {
                await _service.CreateEventAsync(ev, posterFile);
                TempData["Success"] = "Събитието беше добавено успешно!";
            }
            catch
            {
                TempData["Error"] = "Възникна грешка при добавянето на събитието.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ev = await _service.GetEventByIdAsync(id);
            if (ev == null)
            {
                TempData["Error"] = "Събитието не беше намерено.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _service.GetCategoriesSelectListAsync(ev.CategoryId);
            return View(ev);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Event ev, IFormFile? posterFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = "Моля, проверете въведените данни.";
                ViewBag.Categories = await _service.GetCategoriesSelectListAsync(ev.CategoryId);
                return View(ev);
            }

            try
            {
                await _service.UpdateEventAsync(id, ev, posterFile);
                TempData["Success"] = "Събитието беше обновено успешно!";
            }
            catch
            {
                TempData["Error"] = "Възникна грешка при обновяването.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _service.GetEventByIdAsync(id);
            if (ev == null)
            {
                TempData["Error"] = "Събитието не беше намерено.";
                return RedirectToAction(nameof(Index));
            }

            return View(ev);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _service.SoftDeleteEventAsync(id);
                TempData["Success"] = "Събитието беше изтрито успешно!";
            }
            catch
            {
                TempData["Error"] = "Възникна грешка при изтриването.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Dashboard()
        {
            var model = await _service.GetDashboardDataAsync();
            

            return View(model);
        }
    }
}
