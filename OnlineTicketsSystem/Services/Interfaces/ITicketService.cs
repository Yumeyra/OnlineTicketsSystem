using OnlineTicketsSystem.Models;

namespace OnlineTicketsSystem.Services.Interfaces
{
    public interface  ITicketService
    {
        Task<List<Ticket>> GetUserPaidTicketsAsync(string userId);
        Task<byte[]?> GenerateTicketPdfAsync(int ticketId, string userId);
    }
}
