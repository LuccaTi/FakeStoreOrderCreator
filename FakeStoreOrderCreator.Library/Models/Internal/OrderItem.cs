using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreOrderCreator.Library.Models.Internal
{
    public class OrderItem
    {
        public string? Title { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Image { get; set; }
        public int Quantity { get; set; }
    }
}
