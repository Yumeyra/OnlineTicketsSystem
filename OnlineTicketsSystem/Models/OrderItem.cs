using OnlineTicketsSystem.Models.Common;

namespace OnlineTicketsSystem.Models
{
    public class OrderItem : ISoftDeletable
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }  
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
