using OnlineTicketsSystem.Models;
namespace OnlineTicketsSystem.ViewModels
{
    public class HomeIndexVm
    {
        public List<Event> UpcomingEvents { get; set; } = new();
        public List<string> Cities { get; set; } = new();
        public List<string> Categories { get; set; } = new(); // само имената

        // избрани стойности (по желание)
        public string? SelectedCategory { get; set; }
        public string? SelectedCity { get; set; }
        public string? SelectedPrice { get; set; }
        public string? SelectedDate { get; set; }
        public string? CitySearch { get; set; }

    }
}
