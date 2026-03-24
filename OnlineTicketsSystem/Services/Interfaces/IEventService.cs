using OnlineTicketsSystem.ViewModels;
using System.Threading.Tasks;

namespace OnlineTicketsSystem.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventsIndexViewModel> GetEventsIndexAsync(
            string? searchTerm,
            string? category,
            string? city,
            DateTime? date,
            int page);

        Task<EventDetailsViewModel?> GetEventDetailsAsync(int id, string? userId);
    }
}
