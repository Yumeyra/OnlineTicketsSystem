using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Data;
using OnlineTicketsSystem.Helpers;
using OnlineTicketsSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using OnlineTicketsSystem.Models;


namespace OnlineTicketsSystem.Controllers
{
    
    public class CartController : Controller
    {
        private const string CartKey = "CART";
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: /Cart
        [HttpGet]
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<CartVm>(CartKey) ?? new CartVm();
            return View(cart);
        }

        // POST: /Cart/Add
        // ВАЖНО: само логнати могат да добавят
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int eventId, int quantity)
        {
            if (quantity < 1) quantity = 1;
            if (quantity > 20) quantity = 20;

            var ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            if (ev == null) return NotFound();

            var cart = HttpContext.Session.GetObject<CartVm>(CartKey) ?? new CartVm();

            var existing = cart.Items.FirstOrDefault(i => i.EventId == eventId);
            if (existing == null)
            {
                cart.Items.Add(new CartItemVm
                {
                    EventId = ev.Id,
                    Title = ev.Title,
                    Price = ev.Price,
                    Quantity = quantity,
                    ImageUrl = ev.ImageUrl
                });
            }
            else
            {
                existing.Quantity += quantity;
                if (existing.Quantity > 20) existing.Quantity = 20;
            }
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Challenge();

            // ако вече има НЕплатен билет за това събитие -> увеличаваме количеството
            var pending = await _context.Tickets
                .FirstOrDefaultAsync(t => t.UserId == userId && t.EventId == eventId && !t.IsPaid);

            if (pending == null)
            {
                pending = new Ticket
                {
                    UserId = userId,
                    EventId = eventId,
                    Quantity = quantity,
                    UnitPrice = ev.Price,
                    PurchaseDate = DateTime.Now,
                    IsPaid = false,
                    PaidAt = null
                };
                _context.Tickets.Add(pending);
            }
            else
            {
                pending.Quantity += quantity;
                if (pending.Quantity > 20) pending.Quantity = 20;
            }

            await _context.SaveChangesAsync();


            HttpContext.Session.SetObject(CartKey, cart);
            TempData["Message"] = $"Добавено в кошницата: {ev.Title} (x{quantity})";
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int eventId)
        {
            // 1) махаме от session кошницата
            var cart = HttpContext.Session.GetObject<CartVm>(CartKey) ?? new CartVm();
            cart.Items.RemoveAll(i => i.EventId == eventId);
            HttpContext.Session.SetObject(CartKey, cart);

            // 2) soft delete на НЕплатения ticket в базата
            var userId = _userManager.GetUserId(User);
            if (!string.IsNullOrEmpty(userId))
            {
                var pending = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.UserId == userId && t.EventId == eventId && !t.IsPaid && !t.IsDeleted);

                if (pending != null)
                {
                    _context.Tickets.Remove(pending); // => ще стане soft delete заради ApplySoftDelete()
                    await _context.SaveChangesAsync();
                }
            }

            TempData["Message"] = "Премахнато от кошницата.";
            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            // 1) чистим session кошницата
            HttpContext.Session.Remove(CartKey);

            // 2) soft delete на всички НЕплатени tickets за този user
            var userId = _userManager.GetUserId(User);
            if (!string.IsNullOrEmpty(userId))
            {
                var pendingTickets = await _context.Tickets
                    .Where(t => t.UserId == userId && !t.IsPaid && !t.IsDeleted)
                    .ToListAsync();

                if (pendingTickets.Any())
                {
                    _context.Tickets.RemoveRange(pendingTickets); // => soft delete
                    await _context.SaveChangesAsync();
                }
            }

            TempData["Message"] = "Кошницата е изчистена.";
            return RedirectToAction("Index");
        }


       
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.GetObject<CartVm>(CartKey) ?? new CartVm();

            if (!cart.Items.Any())
            {
                TempData["Message"] = "Кошницата е празна.";
                return RedirectToAction("Index");
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Message"] = "Трябва да влезеш в профила си, за да платиш.";
                return RedirectToAction("Index");
            }

            
            foreach (var item in cart.Items)
            {
                var pending = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.UserId == userId && t.EventId == item.EventId && !t.IsPaid);

                if (pending == null) continue;

                pending.IsPaid = true;
                pending.PaidAt = DateTime.Now;
                pending.UnitPrice = item.Price; // ако цената е сменена, обновяваме
                pending.Quantity = item.Quantity;
            }

            await _context.SaveChangesAsync();


            // чистим кошницата
            HttpContext.Session.Remove(CartKey);

            TempData["Message"] = "Плащането е успешно (симулация). Билетите са закупени.";
            return RedirectToAction("Index");
        }


       
    }
}


