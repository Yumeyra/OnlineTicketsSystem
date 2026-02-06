using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


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


        public string? ImageUrl { get; set; }

        

        [Required]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }



        public int? CityId { get; set; }
        public City? CityEntity { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 999999)]
        public decimal Price { get; set; }  // цена в EUR

    }
}
