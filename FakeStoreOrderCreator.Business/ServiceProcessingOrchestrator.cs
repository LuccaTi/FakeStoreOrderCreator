using FakeStoreOrderCreator.Business.Configuration;
using FakeStoreOrderCreator.Business.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeStoreOrderCreator.Library.Models.Api;
using FakeStoreOrderCreator.Library.Models.Internal;
using FakeStoreOrderCreator.Business.Services;
using FakeStoreOrderCreator.Business.Interfaces;

namespace FakeStoreOrderCreator.Business
{
    public class ServiceProcessingOrchestrator : IServiceProcessingOrchestrator
    {
        #region Attributes
        private const string _className = "ServiceProcessingOrchestrator";
        private readonly int _timerFiles;
        private readonly AutoResetEvent _autoResetEvent;
        private bool _disposed = false;
        private readonly object _disposeLock = new object();
        #endregion

        #region Properties
        public Func<bool> IsRunningFunc { get; set; } = () => false;
        #endregion

        #region Dependencies
        private readonly IServiceProcessingEngine _serviceProcessingEngine;
        #endregion

        public ServiceProcessingOrchestrator(IServiceProcessingEngine serviceProcessingEngine)
        {
            try
            {
                _timerFiles = Config.Interval;
                _autoResetEvent = new AutoResetEvent(false);
                _serviceProcessingEngine = serviceProcessingEngine;
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "ServiceProcessingOrchestrator constructor", $"Error: {ex.Message}");
                throw;
            }
        }

        public void EventHandler()
        {
            while (IsRunningFunc())
            {
                try
                {
                    _serviceProcessingEngine.ProcessOrders();
                }
                catch (Exception ex)
                {
                    Logger.Error(_className, "EventHandler", $"Error: {ex.Message}");
                }
                finally
                {
                    _autoResetEvent.WaitOne(TimeSpan.FromSeconds(_timerFiles));
                }
            }

            lock (_disposeLock)
            {
                if (!_disposed)
                {
                    _autoResetEvent.Dispose();
                    _disposed = true;
                }
            }
        }

        public void SignalStop()
        {
            try
            {
                lock (_disposeLock)
                {
                    if (!_disposed)
                    {
                        _autoResetEvent.Set();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "SignalStop", $"Error: {ex.Message}");
                throw;
            }
        }
    }
}
