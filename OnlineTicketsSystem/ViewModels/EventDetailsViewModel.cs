using OnlineTicketsSystem.Models;
namespace OnlineTicketsSystem.ViewModels
{
    public class EventDetailsViewModel
    {
        //public Event Event { get; set; } = null!;
        //public int SoldTickets { get; set; }
        //public int RemainingSeats { get; set; }
        //public bool IsSoldOut => RemainingSeats <= 0;
        //public int Id { get; set; }
        //public decimal Price { get; set; }
        //public string? ImageUrl { get; set; }
        //public string Title { get; set; } = "";
        //public string City { get; set; } = "";
        //public string Venue { get; set; } = "";

        //public DateTime Date { get; set; }
        //public int Capacity { get; set; }

        //public string? Description { get; set; }




        //public string? CategoryName { get; set; }
        public Event Event { get; set; } = null!;

        public int SoldTickets { get; set; }
        public int RemainingSeats { get; set; }
        public bool IsSoldOut => RemainingSeats <= 0;
    }
}
