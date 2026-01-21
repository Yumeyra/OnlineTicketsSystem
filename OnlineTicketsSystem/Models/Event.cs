using System;
using System.ComponentModel.DataAnnotations;


namespace OnlineTicketsSystem.Models
{
    public class Event
    {
      
        
            public int Id { get; set; }

            [Required]
            public string Title { get; set; }

            public string Description { get; set; }

            [Required]
            public string City { get; set; }

            [Required]
            public string Venue { get; set; }

            [Required]
            public DateTime Date { get; set; }

            
        [Required]
        [Range(1, 100000)]
        public int Capacity { get; set; }


            public string ImageUrl { get; set; }

            // Category
            public int CategoryId { get; set; }
            public Category Category { get; set; }
        
    }
}
