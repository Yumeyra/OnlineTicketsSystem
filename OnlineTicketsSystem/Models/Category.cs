using OnlineTicketsSystem.Models.Common;
using System;
using System.ComponentModel.DataAnnotations;


namespace OnlineTicketsSystem.Models
{
    public class Category : ISoftDeletable
    {
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public int Id { get; set; }

            [Required]
            public string Name { get; set; } = null!;



    }
}
