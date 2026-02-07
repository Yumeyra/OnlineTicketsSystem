using OnlineTicketsSystem.Models.Common;
using System.ComponentModel.DataAnnotations;
namespace OnlineTicketsSystem.Models
{
    public class City : ISoftDeletable
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;   

        [Required]
        public string Slug { get; set; } = null!;

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
