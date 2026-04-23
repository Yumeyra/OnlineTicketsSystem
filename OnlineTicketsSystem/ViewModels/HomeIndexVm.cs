using OnlineTicketsSystem.Models;


namespace OnlineTicketsSystem.ViewModels
{
    public class HomeIndexVm
    {
       
        public List<Event> UpcomingEvents { get; set; } = new();

     
        public List<City> Cities { get; set; } = new();

     
        public List<Category> Categories { get; set; } = new();

      
        public int? SelectedCategoryId { get; set; }
        public string? City { get; set; }
        public string? DateRange { get; set; }
        public string? PriceRange { get; set; }
        public List<string> Regions { get; set; } = new();
        public string? Region { get; set; }
        public List<string> CitiesInRegion { get; set; } = new();
        public string? SelectedRegion { get; set; }
        public string? SelectedCity { get; set; }


    }
}
