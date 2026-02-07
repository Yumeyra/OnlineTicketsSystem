using System.Collections.Generic;
using System.Linq;

namespace OnlineTicketsSystem.ViewModels
{
    public class CartVm
    {
        public List<CartItemVm> Items { get; set; } = new();

        public int TotalQuantity => Items.Sum(i => i.Quantity);
        public decimal TotalPrice => Items.Sum(i => i.LineTotal);
    }
}
