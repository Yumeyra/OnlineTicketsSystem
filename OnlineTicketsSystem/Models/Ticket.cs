using Microsoft.AspNetCore.Identity;
using OnlineTicketsSystem.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace OnlineTicketsSystem.Models
{
    public class Ticket : ISoftDeletable
    {
        public int Id { get; set; }

        // Event
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        // User
        public string UserId { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;

        [Range(1, 20)]
        public int Quantity { get; set; } = 1;

        // НОВО: цена за 1 билет (EUR)
        [Range(0, 9999)]
        public decimal UnitPrice { get; set; }

        // НОВО: платено/неплатено (ние ще използваме “симулация”)
        //public bool IsPaid { get; set; } = true;
        public bool IsPaid { get; set; } = false;
        public DateTime? PaidAt { get; set; } = null;


        //public DateTime? PaidAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;


        //public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

        // Soft delete (ако го правиш навсякъде)

    }
}
