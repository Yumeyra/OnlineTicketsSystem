using OnlineTicketsSystem.Data;
using OnlineTicketsSystem.Helpers;
using OnlineTicketsSystem.Models;
using Microsoft.EntityFrameworkCore;

using OnlineTicketsSystem.Services.Interfaces;

namespace OnlineTicketsSystem.Services
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;

        public TicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ticket>> GetUserPaidTicketsAsync(string userId)
        {
            return await _context.Tickets
                .Where(t => t.UserId == userId && t.IsPaid)
                .Include(t => t.Event)
                    .ThenInclude(e => e.Category)
                .OrderByDescending(t => t.PaidAt ?? t.PurchaseDate)
                .ToListAsync();
        }

        public async Task<byte[]?> GenerateTicketPdfAsync(int ticketId, string userId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Event)
                    .ThenInclude(e => e.Category)
                .FirstOrDefaultAsync(t => t.Id == ticketId && t.UserId == userId);

            if (ticket == null || !ticket.IsPaid)
                return null;

            var pdfBytes = TicketPdfDocument.Generate(
                ticket.Event.Title,
                ticket.Event.Category?.Name ?? "Без категория",
                ticket.Event.City,
                ticket.Event.Venue,
                ticket.Event.Date,
                ticket.Quantity,
                ticket.UnitPrice,
                ticket.UnitPrice * ticket.Quantity,
                ticket.PaidAt,
                $"TICKET-{ticket.Id}"
            );

            return pdfBytes;
        }
    }
}
