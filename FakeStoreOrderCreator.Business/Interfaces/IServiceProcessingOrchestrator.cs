using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreOrderCreator.Business.Interfaces
{
    public interface IServiceProcessingOrchestrator
    {
        Func<bool> IsRunningFunc { get; set; }
        public void EventHandler();
        public void SignalStop();
    }
}
