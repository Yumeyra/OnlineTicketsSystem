using OnlineTicketsSystem.Data;
using OnlineTicketsSystem.Models;
using OnlineTicketsSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace OnlineTicketsSystem.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CanReviewAsync(string userId, int eventId)
        {
            // Съществува ли събитието
            var evExists = await _context.Events.AnyAsync(e => e.Id == eventId);
            if (!evExists) return false;

            // Само с платен билет
            var hasPaidTicket = await _context.Tickets.AnyAsync(t =>
                t.UserId == userId &&
                t.EventId == eventId &&
                t.IsPaid);

            if (!hasPaidTicket) return false;

            // Вече оставен отзив?
            var already = await _context.Reviews
                .IgnoreQueryFilters()
                .AnyAsync(r => r.UserId == userId && r.EventId == eventId && !r.IsDeleted);

            return !already;
        }

        public async Task<string> CreateReviewAsync(string userId, int eventId, int rating, string? comment)
        {
            var evExists = await _context.Events.AnyAsync(e => e.Id == eventId);
            if (!evExists) return "Събитието не съществува.";

            // Само с платен билет
            var hasPaidTicket = await _context.Tickets.AnyAsync(t =>
                t.UserId == userId &&
                t.EventId == eventId &&
                t.IsPaid);

            if (!hasPaidTicket)
                return "Можеш да оставиш отзив само ако имаш платен билет за това събитие.";

            // Вече оставен отзив?
            var already = await _context.Reviews
                .IgnoreQueryFilters()
                .AnyAsync(r => r.UserId == userId && r.EventId == eventId && !r.IsDeleted);

            if (already)
                return "Вече си оставил/а отзив за това събитие.";

            if (rating < 1) rating = 1;
            if (rating > 5) rating = 5;

            var review = new Review
            {
                UserId = userId,
                EventId = eventId,
                Rating = rating,
                Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsApproved = true // ако искаш админ одобрение => false
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return "Благодарим! Отзивът е добавен.";
        }
    }
}

