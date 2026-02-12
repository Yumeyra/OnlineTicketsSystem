using OnlineTicketsSystem.Models;
using System.Collections.Generic;
namespace OnlineTicketsSystem.ViewModels
{
    public class EventsIndexViewModel
    {
        public List<Event> Upcoming { get; set; } = new();
        public List<Event> Past { get; set; } = new();
    }
}
