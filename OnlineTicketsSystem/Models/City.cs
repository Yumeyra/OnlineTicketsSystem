using System.ComponentModel.DataAnnotations;
namespace OnlineTicketsSystem.Models
{
    public class City
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;   

        [Required]
        public string Slug { get; set; } = null!;   
    }
}
