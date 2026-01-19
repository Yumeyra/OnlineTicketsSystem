using Microsoft.AspNetCore.Identity;

namespace OnlineTicketsSystem.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public DateTime PurchaseDate { get; set; }

        // Event
        public int EventId { get; set; }
        public Event Event { get; set; }

        // User
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
