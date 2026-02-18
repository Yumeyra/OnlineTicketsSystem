using OnlineTicketsSystem.Models;
namespace OnlineTicketsSystem.ViewModels
{
    public class EventDetailsViewModel
    {
        
        public Event Event { get; set; } = null!;

        public int SoldTickets { get; set; }
        public int RemainingSeats { get; set; }
        public bool IsSoldOut => RemainingSeats <= 0;
    }
}
