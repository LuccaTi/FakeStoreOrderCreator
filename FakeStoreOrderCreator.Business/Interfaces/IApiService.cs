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
        public Task<List<Cart>> GetCartsAsync(CancellationToken cancellationToken);
        public Task<List<Product>> GetProductsAsync(CancellationToken cancellationToken);
        public Task<List<User>> GetUsersAsync(CancellationToken cancellationToken);
    }
}
