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
        private readonly int _timerSeconds;
        private readonly CancellationTokenSource _cancellationTokenSource;
        #endregion


        #region Dependencies
        private readonly IServiceProcessingEngine _serviceProcessingEngine;
        #endregion

        public ServiceProcessingOrchestrator(IServiceProcessingEngine serviceProcessingEngine)
        {
            try
            {
                _timerSeconds = Config.Interval;
                _cancellationTokenSource = new CancellationTokenSource();
                _serviceProcessingEngine = serviceProcessingEngine;
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "ServiceProcessingOrchestrator constructor", $"Error: {ex.Message}");
                throw;
            }
        }

        public async Task EventHandlerAsync()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    await _serviceProcessingEngine.ProcessOrdersAsync();
                }
                catch (Exception ex)
                {
                    Logger.Error(_className, "EventHandlerAsync", $"Error: {ex.Message}");
                }
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_timerSeconds), _cancellationTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        public void SignalStop()
        {
            try
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
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
