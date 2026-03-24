using OnlineTicketsSystem.Data;
using OnlineTicketsSystem.Services.Interfaces;
using OnlineTicketsSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace OnlineTicketsSystem.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EventsIndexViewModel> GetEventsIndexAsync(
            string? searchTerm,
            string? category,
            string? city,
            DateTime? date,
            int page)
        {
            const int pageSize = 6;

            var query = _context.Events
                .Include(e => e.Category)
                .AsQueryable();

            // 🔍 Filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(e =>
                    e.Title.Contains(term) ||
                    e.City.Contains(term) ||
                    e.Venue.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                var cat = category.Trim();
                query = query.Where(e => e.Category != null && e.Category.Name == cat);
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                var c = city.Trim();
                query = query.Where(e => e.City.Contains(c));
            }

            if (date.HasValue)
            {
                var d = date.Value.Date;
                query = query.Where(e => e.Date.Date == d);
            }

            var today = DateTime.Today;

            // 📍 Cities
            var cities = await _context.Events
                .Select(e => e.City)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            // 🏷️ Categories
            var categories = await _context.Categories
                .Select(c => c.Name)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync();

            // 📅 Upcoming & Past
            var upcomingQuery = query
                .Where(e => e.Date.Date >= today)
                .OrderBy(e => e.Date);

            var pastQuery = query
                .Where(e => e.Date.Date < today)
                .OrderByDescending(e => e.Date);

            var totalUpcoming = await upcomingQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalUpcoming / (double)pageSize);

            if (totalPages == 0) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var upcoming = await upcomingQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var past = await pastQuery
                .Take(6)
                .ToListAsync();

            return new EventsIndexViewModel
            {
                Upcoming = upcoming,
                Past = past,

                SearchTerm = searchTerm,
                SelectedCity = city,
                SelectedCategory = category,
                SelectedDate = date,

                Cities = cities,
                Categories = categories,

                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        public async Task<EventDetailsViewModel?> GetEventDetailsAsync(int id, string? userId)
        {
            var ev = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return null;

            var soldTickets = await _context.Tickets
                .Where(t => t.EventId == id)
                .SumAsync(t => (int?)t.Quantity) ?? 0;

            var remainingSeats = ev.Capacity - soldTickets;

            var reviewsQuery = _context.Reviews
                .Include(r => r.User)
                .Where(r => r.EventId == id && r.IsApproved);

            var reviewsCount = await reviewsQuery.CountAsync();
            var averageRating = reviewsCount == 0
                ? 0
                : await reviewsQuery.AverageAsync(r => (double)r.Rating);

            var reviews = await reviewsQuery
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            bool canReview = false;
            bool userHasReviewed = false;

            if (!string.IsNullOrEmpty(userId))
            {
                canReview = await _context.Tickets.AnyAsync(t =>
                    t.UserId == userId &&
                    t.EventId == id &&
                    t.IsPaid);

                userHasReviewed = await _context.Reviews
                    .IgnoreQueryFilters()
                    .AnyAsync(r => r.UserId == userId && r.EventId == id && !r.IsDeleted);
            }

            return new EventDetailsViewModel
            {
                Event = ev,
                SoldTickets = soldTickets,
                RemainingSeats = remainingSeats,
                Reviews = reviews,
                ReviewsCount = reviewsCount,
                AverageRating = averageRating,
                CanReview = canReview,
                UserHasReviewed = userHasReviewed
            };
        }
    }
}
