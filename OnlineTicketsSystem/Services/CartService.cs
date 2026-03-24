using OnlineTicketsSystem.Data;
using OnlineTicketsSystem.Helpers;
using OnlineTicketsSystem.Models;
using OnlineTicketsSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Services.Interfaces;


namespace OnlineTicketsSystem.Services
{
    public class CartService : ICartService
    {
        private const string CartKey = "CART";
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public CartVm GetCart(ISession session)
        {
            return session.GetObject<CartVm>(CartKey) ?? new CartVm();
        }

        public async Task<string?> AddToCartAsync(
            ISession session,
            string userId,
            int eventId,
            int quantity)
        {
            if (quantity < 1) quantity = 1;
            if (quantity > 20) quantity = 20;

            var ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            if (ev == null) return "Събитието не е намерено.";

            if (ev.Date.Date < DateTime.Today)
                return "Събитието е минало.";

            var cart = GetCart(session);

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
                existing.Quantity = Math.Min(20, existing.Quantity + quantity);
            }

            var pending = await _context.Tickets
                .FirstOrDefaultAsync(t => t.UserId == userId && t.EventId == eventId && !t.IsPaid);

            if (pending == null)
            {
                _context.Tickets.Add(new Ticket
                {
                    UserId = userId,
                    EventId = eventId,
                    Quantity = quantity,
                    UnitPrice = ev.Price,
                    PurchaseDate = DateTime.Now,
                    IsPaid = false
                });
            }
            else
            {
                pending.Quantity = Math.Min(20, pending.Quantity + quantity);
            }

            await _context.SaveChangesAsync();

            session.SetObject(CartKey, cart);
            return null;
        }

        public async Task RemoveFromCartAsync(ISession session, string userId, int eventId)
        {
            var cart = GetCart(session);
            cart.Items.RemoveAll(i => i.EventId == eventId);
            session.SetObject(CartKey, cart);

            var pending = await _context.Tickets
                .FirstOrDefaultAsync(t => t.UserId == userId && t.EventId == eventId && !t.IsPaid && !t.IsDeleted);

            if (pending != null)
            {
                _context.Tickets.Remove(pending);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(ISession session, string userId)
        {
            session.Remove(CartKey);

            var tickets = await _context.Tickets
                .Where(t => t.UserId == userId && !t.IsPaid && !t.IsDeleted)
                .ToListAsync();

            if (tickets.Any())
            {
                _context.Tickets.RemoveRange(tickets);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string?> CheckoutAsync(ISession session, string userId)
        {
            var cart = GetCart(session);

            if (!cart.Items.Any())
                return "Кошницата е празна.";

            foreach (var item in cart.Items)
            {
                var ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == item.EventId);
                if (ev == null)
                    return "Събитие липсва.";

                var sold = await _context.Tickets
                    .Where(t => t.EventId == item.EventId)
                    .SumAsync(t => (int?)t.Quantity) ?? 0;

                var remaining = ev.Capacity - sold;

                if (item.Quantity > remaining)
                    return $"Недостатъчни места за {ev.Title}.";
            }

            foreach (var item in cart.Items)
            {
                _context.Tickets.Add(new Ticket
                {
                    UserId = userId,
                    EventId = item.EventId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    IsPaid = true,
                    PaidAt = DateTime.UtcNow,
                    PurchaseDate = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            session.Remove(CartKey);

            return null;
        }
    }
}

