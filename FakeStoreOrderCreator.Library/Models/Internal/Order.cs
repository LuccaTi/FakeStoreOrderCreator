using FakeStoreOrderCreator.Library.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreOrderCreator.Library.Models.Internal
{
    public class Order
    {
        public string? OrderGuid { get; set; }
        public Customer? Customer { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public DateTime OrderDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public DateTime DeliveredDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public ShippingStatus ShippingStatus { get; set; }
    }
}
