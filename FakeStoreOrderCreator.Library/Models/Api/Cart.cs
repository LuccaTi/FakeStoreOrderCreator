using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreOrderCreator.Library.Models.Api
{
    public class Cart
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime? Date { get; set; }
        public Products[]? Products { get; set; }
    }
    public class Products
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
