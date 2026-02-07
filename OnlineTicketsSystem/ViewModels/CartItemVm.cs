namespace OnlineTicketsSystem.ViewModels
{
    public class CartItemVm
    {
        public int EventId { get; set; }
        public string Title { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }

        public decimal LineTotal => Price * Quantity;
    }
}
