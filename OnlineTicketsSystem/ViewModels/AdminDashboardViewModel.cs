namespace OnlineTicketsSystem.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalEvents { get; set; }
        public int TotalSoldTickets { get; set; }
        public decimal TotalRevenue { get; set; }
        public int UpcomingEvents { get; set; }
        public int PastEvents { get; set; }
        public int SoldOutEvents { get; set; }

        public List<TopEventViewModel> TopEvents { get; set; } = new();
        public List<RecentPurchaseViewModel> RecentPurchases { get; set; } = new();
        public List<SoldOutEventViewModel> SoldOutEventList { get; set; } = new();
    }
    public class TopEventViewModel
    {
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int SoldTickets { get; set; }
        public decimal Revenue { get; set; }
        public int Capacity { get; set; }
        public int RemainingSeats { get; set; }
    }
    public class RecentPurchaseViewModel
    {
        public int TicketId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    public class SoldOutEventViewModel
    {
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int Capacity { get; set; }
        public int SoldTickets { get; set; }
    }

}
