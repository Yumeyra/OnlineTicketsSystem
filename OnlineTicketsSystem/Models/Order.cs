using Microsoft.AspNetCore.Identity;
using OnlineTicketsSystem.Models.Common;
namespace OnlineTicketsSystem.Models
{
    public class Order : ISoftDeletable
    {
        public int Id { get; set; }

        public string UserId { get; set; } = "";
        public IdentityUser User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // обща сума (по избор)
        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; } = true;
        public DateTime? PaidAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem> Items { get; set; } = new();

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt
        {
            get; set;
        }
    }
}
