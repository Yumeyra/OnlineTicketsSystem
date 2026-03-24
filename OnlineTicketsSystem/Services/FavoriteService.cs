using OnlineTicketsSystem.Data;
using OnlineTicketsSystem.Models;
using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Services.Interfaces;
namespace OnlineTicketsSystem.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly ApplicationDbContext _context;

        public FavoriteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Favorite>> GetUserFavoritesAsync(string userId)
        {
            return await _context.Favorites
                .Include(f => f.Event)
                    .ThenInclude(e => e.Category)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<string> AddFavoriteAsync(string userId, int eventId)
        {
            var exists = await _context.Events.AnyAsync(e => e.Id == eventId);
            if (!exists) return "Събитието не съществува.";

            var existing = await _context.Favorites
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.EventId == eventId);

            if (existing == null)
            {
                _context.Favorites.Add(new Favorite
                {
                    UserId = userId,
                    EventId = eventId,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
                await _context.SaveChangesAsync();
                return "Добавено в любими.";
            }

            if (existing.IsDeleted)
            {
                existing.IsDeleted = false;
                existing.DeletedAt = null;
                await _context.SaveChangesAsync();
                return "Добавено в любими.";
            }

            return "Това събитие вече е в любими.";
        }

        public async Task<string> RemoveFavoriteAsync(string userId, int eventId)
        {
            var fav = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.EventId == eventId);

            if (fav == null) return "Не е намерено в любими.";

            fav.IsDeleted = true;
            fav.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return "Премахнато от любими.";
        }

        public async Task<string> ToggleFavoriteAsync(string userId, int eventId)
        {
            var exists = await _context.Events.AnyAsync(e => e.Id == eventId);
            if (!exists) return "Събитието не съществува.";

            var fav = await _context.Favorites
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.EventId == eventId);

            if (fav == null)
            {
                _context.Favorites.Add(new Favorite
                {
                    UserId = userId,
                    EventId = eventId,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
                await _context.SaveChangesAsync();
                return "Добавено в любими.";
            }

            if (fav.IsDeleted)
            {
                fav.IsDeleted = false;
                fav.DeletedAt = null;
                await _context.SaveChangesAsync();
                return "Добавено в любими.";
            }

            fav.IsDeleted = true;
            fav.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return "Премахнато от любими.";
        }
    }
}

