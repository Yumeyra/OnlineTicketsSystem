using Microsoft.AspNetCore.Identity;
using OnlineTicketsSystem.Models.Common;
namespace OnlineTicketsSystem.Models
{
    public class Favorite : ISoftDeletable
    {
        public int Id { get; set; }

        public string UserId { get; set; } = "";
        public IdentityUser User { get; set; } = null!;

        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
