using OnlineTicketsSystem.Models;

namespace OnlineTicketsSystem.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<List<Favorite>> GetUserFavoritesAsync(string userId);
        Task<string> AddFavoriteAsync(string userId, int eventId);
        Task<string> RemoveFavoriteAsync(string userId, int eventId);
        Task<string> ToggleFavoriteAsync(string userId, int eventId);
    }
}
