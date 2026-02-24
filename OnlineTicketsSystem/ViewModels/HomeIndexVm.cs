using OnlineTicketsSystem.Models;
namespace OnlineTicketsSystem.ViewModels
{
    public class HomeIndexVm
    {
       
        public List<Event> UpcomingEvents { get; set; } = new();

        // Градове (за секцията "Популярни градове")
        public List<string> Cities { get; set; } = new();

        // Категории (за dropdown)
        public List<Category> Categories { get; set; } = new();

        // Запазване на избора (за формата)
        public int? SelectedCategoryId { get; set; }
        public string? City { get; set; }
        public string? DateRange { get; set; }
        public string? PriceRange { get; set; }


    }
}
