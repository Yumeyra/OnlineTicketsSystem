using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using OnlineTicketsSystem.Models.Common;

namespace OnlineTicketsSystem.Models
{
    public class Review :ISoftDeletable
    {
        public int Id { get; set; }

        public string UserId { get; set; } = "";
        public IdentityUser User { get; set; } = null!;

        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; } = 5;

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
        public bool IsApproved { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
