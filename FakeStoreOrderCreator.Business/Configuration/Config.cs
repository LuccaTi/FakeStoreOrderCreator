using FakeStoreOrderCreator.Business.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreOrderCreator.Business.Configuration
{
    public static class Config
    {
        #region Attributes
        private const string _className = "Config";
        private static IConfiguration? _config;
        private static string? _fakeStoreDirectory;
        private static string? _apiUrl;
        private static int _interval;
        private static bool _writeLogConsole;
        #endregion

        #region Properties
        public static string FakeStoreDirectory
        {
            get { return _fakeStoreDirectory!; }
        }
        public static string ApiUrl
        {
            get { return _apiUrl!; }
        }
        public static int Interval
        {
            get { return _interval; }
        }
        public static bool WriteLogConsole
        {
            get { return _writeLogConsole; }
        }
        #endregion

        #region Methods
        public static void LoadConfig()
        {
            try
            {
                _config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                _writeLogConsole = Convert.ToBoolean(_config["AppConfig:WriteLogConsole"]);
                
                string logDirectory = _config["AppLogging:LogDirectory"] ?? "logs".Replace(@"/", "\\");
                Logger.InitLogger(logDirectory);
                Logger.Info("Logger initialized, loading settings...");

                _fakeStoreDirectory = _config["AppConfig:FakeStoreDirectory"] ?? throw new Exception("Appsettings.json parameter: \"FakeStoreDirecory\" was not filled!");
                Logger.Debug(_className, "LoadConfig", $"FakeStoreDirectory: {_fakeStoreDirectory}");

                _apiUrl = _config["AppConfig:ApiUrl"] ?? throw new Exception("Appsettings.json parameter: \"ApiUrl\" was not filled!");
                Logger.Debug(_className, "LoadConfig", $"ApiUrl: {_apiUrl}");

                int interval = Convert.ToInt32(_config["AppConfig:Interval"]);
                _interval = interval == 0 ? 60 : interval;
                Logger.Debug(_className, "LoadConfig", $"Interval: {_interval} seconds");
                
                Logger.Debug(_className, "LoadConfig", $"WriteLogConsole: {_writeLogConsole}");

                Logger.Info("Settings loaded!");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while loading application settings!", ex);
            }
        }
        private static void CreateDirectories()
        {
            try
            {
                if (!Directory.Exists(_fakeStoreDirectory))
                {
                    Logger.Debug(_className, "CreateDirectories", $"Creating directory: {_fakeStoreDirectory}");
                    Directory.CreateDirectory(_fakeStoreDirectory!);
                }
                else
                {
                    Logger.Debug(_className, "CreateDirectories", $"Directory: {_fakeStoreDirectory} already Exists!");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "CreateDirectories", $"{ex.Message}");
                throw;
            }
        }
        public static string Get(string parameter)
        {
            try
            {
                return _config?[parameter]!;
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Get", $"{ex.Message}");
                throw;
            }
        }
        #endregion
    }
}
