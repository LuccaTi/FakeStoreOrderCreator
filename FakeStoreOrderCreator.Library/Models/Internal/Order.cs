using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreOrderCreator.Library.Models.Internal
{
    public class Order
    {
        public Customer? Customer { get; set; }
        public List<OrderItems> Items { get; set; } = new List<OrderItems>();
    }
}
