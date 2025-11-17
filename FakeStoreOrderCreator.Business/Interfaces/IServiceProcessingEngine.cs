using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreOrderCreator.Business.Interfaces
{
    public interface IServiceProcessingEngine
    {
        public Task ProcessOrdersAsync(CancellationToken cancellationToken);
    }
}
