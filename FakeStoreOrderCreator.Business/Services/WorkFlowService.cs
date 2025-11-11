using FakeStoreOrderCreator.Business.Configuration;
using FakeStoreOrderCreator.Business.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreOrderCreator.Business.Services
{
    public class WorkFlowService
    {
        #region Attributes
        private const string _className = "WorkFlowService";
        private readonly int _timerFiles;
        private readonly AutoResetEvent _autoResetEvent;
        private readonly Func<bool> _isRunningFunc;
        private bool _disposed = false;
        private readonly object _disposeLock = new object();
        #endregion

        #region Dependencies
        private ApiService _apiService;
        private FileService _fileService;
        #endregion

        public WorkFlowService(Func<bool> isRunningFunc)
        {
            try
            {
                _timerFiles = Config.Interval;
                _isRunningFunc = isRunningFunc;
                _autoResetEvent = new AutoResetEvent(false);
                _apiService = new ApiService();
                _fileService = new FileService();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "WorkFlowService constructor", $"Error: {ex.Message}");
                throw;
            }
        }

        public void EventHandler()
        {

            while (_isRunningFunc())
            {
                try
                {

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
