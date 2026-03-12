//using OnlineTicketsSystem.Models;
//using System.Collections.Generic;
//namespace OnlineTicketsSystem.ViewModels
//{
//    public class EventsIndexViewModel
//    {
//        public List<Event> Upcoming { get; set; } = new();
//        public List<Event> Past { get; set; } = new();
//    }
//}
using OnlineTicketsSystem.Models;

namespace OnlineTicketsSystem.ViewModels
{
    public class EventsIndexViewModel
    {
        public List<Event> Upcoming { get; set; } = new();
        public List<Event> Past { get; set; } = new();

        public string? SearchTerm { get; set; }
        public string? SelectedCity { get; set; }
        public string? SelectedCategory { get; set; }
        public DateTime? SelectedDate { get; set; }

        public List<string> Cities { get; set; } = new();
        public List<string> Categories { get; set; } = new();

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
