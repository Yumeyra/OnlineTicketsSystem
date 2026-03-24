using OnlineTicketsSystem.Data;
using OnlineTicketsSystem.Services.Interfaces;
using OnlineTicketsSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace OnlineTicketsSystem.Services
{
    public class HomeService : IHomeService
    
    {
        private readonly ApplicationDbContext _context;

        public HomeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HomeIndexVm> GetHomeDataAsync(
            int? categoryId,
            string? selectedRegion,
            string? selectedCity,
            string? dateRange,
            string? priceRange)
        {
            var today = DateTime.Today;
            List<string> citiesInRegion = new();

            // 📍 Cities in region
            if (!string.IsNullOrWhiteSpace(selectedRegion))
            {
                citiesInRegion = await _context.Cities
                    .Where(c => c.Region == selectedRegion && !c.IsDeleted)
                    .Select(c => c.Name)
                    .Distinct()
                    .OrderBy(n => n)
                    .ToListAsync();
            }

            var query = _context.Events
                .Include(e => e.Category)
                .Where(e => e.Date >= today)
                .AsQueryable();

            // 🔍 Filters
            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(selectedCity))
                query = query.Where(e => e.City == selectedCity);

            if (dateRange == "week")
                query = query.Where(e => e.Date <= today.AddDays(7));
            else if (dateRange == "month")
                query = query.Where(e => e.Date <= today.AddMonths(1));

            if (!string.IsNullOrWhiteSpace(priceRange))
            {
                if (priceRange == "0-10")
                    query = query.Where(e => e.Price >= 0 && e.Price <= 10);
                else if (priceRange == "10-20")
                    query = query.Where(e => e.Price > 10 && e.Price <= 20);
                else if (priceRange == "20+")
                    query = query.Where(e => e.Price >= 20);
            }

            // 📅 Upcoming events
            var upcoming = await query
                .OrderBy(e => e.Date)
                .Take(12)
                .ToListAsync();

            // 🏷️ Categories
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            // 🌍 Regions
            var regions = await _context.Cities
                .Select(c => c.Region)
                .Distinct()
                .OrderBy(r => r)
                .ToListAsync();

            // 🔥 Popular cities (fixed order)
            var preferredCityOrder = new List<string>
            {
                "София", "Пловдив", "Варна", "Бургас",
                "Русе", "Стара Загора", "Плевен", "Велико Търново"
            };

            var popularCities = await _context.Cities
                .Where(c => !c.IsDeleted && preferredCityOrder.Contains(c.Name))
                .ToListAsync();

            popularCities = popularCities
                .OrderBy(c => preferredCityOrder.IndexOf(c.Name))
                .ToList();

            // 🖼️ Hero images
            var heroImages = await _context.Events
                .Where(e => !string.IsNullOrWhiteSpace(e.ImageUrl))
                .Select(e => new
                {
                    e.ImageUrl,
                    SoldTickets = _context.Tickets
                        .Where(t => t.EventId == e.Id && t.IsPaid)
                        .Sum(t => (int?)t.Quantity) ?? 0
                })
                .OrderByDescending(x => x.SoldTickets)
                .ThenBy(x => x.ImageUrl)
                .Take(5)
                .Select(x => x.ImageUrl!)
                .ToListAsync();

            if (!heroImages.Any())
            {
                heroImages = await _context.Events
                    .Where(e => !string.IsNullOrWhiteSpace(e.ImageUrl))
                    .OrderByDescending(e => e.Date)
                    .Take(5)
                    .Select(e => e.ImageUrl!)
                    .ToListAsync();
            }

            return new HomeIndexVm
            {
                UpcomingEvents = upcoming,
                Categories = categories,
                Cities = popularCities,

                SelectedCategoryId = categoryId,
                SelectedRegion = selectedRegion,
                SelectedCity = selectedCity,
                DateRange = dateRange,
                PriceRange = priceRange,

                Regions = regions,
                CitiesInRegion = citiesInRegion,

                //HeroImages = heroImages
            };
        }
    }
}
