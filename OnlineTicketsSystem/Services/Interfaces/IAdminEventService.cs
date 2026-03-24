using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineTicketsSystem.Models;
using OnlineTicketsSystem.ViewModels;

namespace OnlineTicketsSystem.Services.Interfaces
{
    public interface IAdminEventService
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<SelectList> GetCategoriesSelectListAsync(int? selectedId = null);

        Task CreateEventAsync(Event ev, IFormFile? posterFile);
        Task<Event?> GetEventByIdAsync(int id);
        Task UpdateEventAsync(int id, Event ev, IFormFile? posterFile);
        Task SoftDeleteEventAsync(int id);

        Task<AdminDashboardViewModel> GetDashboardDataAsync();
    }
}
