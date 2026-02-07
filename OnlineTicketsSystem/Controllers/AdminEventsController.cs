using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineTicketsSystem.Data;
using OnlineTicketsSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace OnlineTicketsSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminEventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminEventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /AdminEvents
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events
                .Include(e => e.Category)
                .OrderBy(e => e.Date)
                .ToListAsync();

            return View(events);
        }

        // GET: /AdminEvents/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: /AdminEvents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event ev)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", ev.CategoryId);
                return View(ev);
            }

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminEvents/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            ViewBag.Categories = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", ev.CategoryId);
            return View(ev);
        }

        // POST: /AdminEvents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event ev)
        {
            if (id != ev.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", ev.CategoryId);
                return View(ev);
            }

            _context.Events.Update(ev);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminEvents/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _context.Events.Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        //// POST: /AdminEvents/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var ev = await _context.Events.FindAsync(id);
        //    if (ev == null) return NotFound();

        //    _context.Events.Remove(ev);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // ВАЖНО: ако имаш query filter, FindAsync може да НЕ намира вече изтрити.
            // За изтриване на активни събития е ок, но аз го правя сигурно:
            var ev = await _context.Events
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return NotFound();

            // Тук НЕ Remove(), а маркираме IsDeleted.
            ev.IsDeleted = true;
            ev.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

