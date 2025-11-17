using FakeStoreOrderCreator.Library.Models.Api;
using FakeStoreOrderCreator.Library.Models.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreOrderCreator.Business.Interfaces
{
    public interface IFileService
    {
        public void CreateProductRegisterFiles(List<Product> products);
        public void CreateOrderFiles(List<Order> orders);
        public void CreatePaymentConfirmedFiles(List<Order> paymentConfirmedOrders);
        public void CreateOrderShippedFiles(List<Order> paymentConfirmedOrders);
        public void CreateOrderDeliveredFiles(List<Order> paymentConfirmedOrders);
    }
}
