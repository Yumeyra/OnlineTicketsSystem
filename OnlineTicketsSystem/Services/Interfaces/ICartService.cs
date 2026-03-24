using OnlineTicketsSystem.ViewModels;

namespace OnlineTicketsSystem.Services.Interfaces
{
    public interface  ICartService
    {
        CartVm GetCart(ISession session);

        Task<string?> AddToCartAsync(
            ISession session,
            string userId,
            int eventId,
            int quantity);

        Task RemoveFromCartAsync(ISession session, string userId, int eventId);

        Task ClearCartAsync(ISession session, string userId);

        Task<string?> CheckoutAsync(ISession session, string userId);
    }
}
