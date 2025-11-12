using FakeStoreOrderCreator.Library.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreOrderCreator.Business.Interfaces
{
    public interface IApiService
    {
        public List<Cart> GetCarts();
        public List<Product> GetProducts();
        public List<User> GetUsers();
    }
}
