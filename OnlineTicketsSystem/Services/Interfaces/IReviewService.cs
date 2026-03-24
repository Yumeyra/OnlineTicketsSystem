namespace OnlineTicketsSystem.Services.Interfaces
{
    public interface  IReviewService
    {
        Task<string> CreateReviewAsync(string userId, int eventId, int rating, string? comment);
        Task<bool> CanReviewAsync(string userId, int eventId);
    }
}
