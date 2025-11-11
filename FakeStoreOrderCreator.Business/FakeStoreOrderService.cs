using FakeStoreOrderCreator.Business.Configuration;
using FakeStoreOrderCreator.Business.Logging;
using FakeStoreOrderCreator.Business.Services;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Reflection.Metadata.Ecma335;
using System.Timers;


namespace FakeStoreOrderCreator.Business
{
    public class FakeStoreOrderService
    {
        #region Attributes
        private const string _className = "FakeStoreOrderService";
        private List<Task> _tasks;
        private bool _isRunning;
        #endregion

        #region Dependencies
        private WorkFlowService _workFlowService;
        #endregion

        public FakeStoreOrderService()
        {
            try
            {
                _isRunning = true;
                _workFlowService = new WorkFlowService(() => _isRunning);
                _tasks = new List<Task>();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "FakeStoreOrderService constructor", $"Error: {ex.ToString()}{Environment.NewLine}Application will be terminated by TopShelf!");
                throw;
            }
        }

        #region Methods
        public void Start()
        {
            try
            {
                Logger.Info("Application started successfully!");
                CreateWorkThreads();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Start", $"Error: {ex.Message}");
                throw;
            }

        }
        private void CreateWorkThreads()
        {
            try
            {
                _tasks.Add(Task.Run(_workFlowService.EventHandler));
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "CreateWorkThreads", $"Error: {ex.Message}");
                throw;
            }
        }
        public void Stop()
        {
            try
            {
                Logger.Info("Request to stop received, stopping application...");
                _isRunning = false;
                _workFlowService.SignalStop();
                Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Stop", $"Error: {ex.Message} - TopShelf will stop the application");
                throw;
            }
        }
        public void Dispose()
        {
            try
            {
                Task.WaitAll(_tasks.ToArray(), TimeSpan.FromSeconds(30));

                foreach (var task in _tasks)
                {
                    task.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Dispose", $"Error: {ex.Message}");
                throw;
            }
        }
        #endregion
    }
}
