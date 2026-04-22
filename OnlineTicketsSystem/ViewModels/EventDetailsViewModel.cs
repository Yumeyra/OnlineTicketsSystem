using OnlineTicketsSystem.Models;
namespace OnlineTicketsSystem.ViewModels
{
    public class EventDetailsViewModel
    {
        
        public Event Event { get; set; } = null!;

        public int SoldTickets { get; set; }
        public int RemainingSeats { get; set; }
       

        // Reviews
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public List<Review> Reviews { get; set; } = new();

        // Permissions
        public bool CanReview { get; set; }          
        public bool UserHasReviewed { get; set; }
        //public TimeSpan StartTime { get; set; }

        public List<Event> RelatedEvents { get; set; } = new();
    }
}
