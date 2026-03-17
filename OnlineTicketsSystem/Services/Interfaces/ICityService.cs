using OnlineTicketsSystem.Models;

namespace OnlineTicketsSystem.Services.Interfaces
{
    public  interface ICityService
    {
        Task<List<City>> GetAllCitiesAsync();
        Task<City?> GetCityBySlugAsync(string slug);
        Task<List<Event>> GetEventsByCityNameAsync(string cityName);
    }
}
