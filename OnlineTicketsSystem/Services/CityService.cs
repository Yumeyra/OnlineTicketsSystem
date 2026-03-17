using OnlineTicketsSystem.Data;
using OnlineTicketsSystem.Models;
using OnlineTicketsSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace OnlineTicketsSystem.Services
{
    public class CityService : ICityService
    {
        private readonly ApplicationDbContext _context;

        public CityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<City>> GetAllCitiesAsync()
        {
            return await _context.Cities
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<City?> GetCityBySlugAsync(string slug)
        {
            return await _context.Cities
                .FirstOrDefaultAsync(c => c.Slug == slug);
        }

        public async Task<List<Event>> GetEventsByCityNameAsync(string cityName)
        {
            return await _context.Events
                .Include(e => e.Category)
                .Where(e => e.City == cityName)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }
    }
}
