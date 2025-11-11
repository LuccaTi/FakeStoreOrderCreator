using FakeStoreOrderCreator.Business;
using FakeStoreOrderCreator.Business.Configuration;
using FakeStoreOrderCreator.Business.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using Topshelf;

namespace FakeStoreOrderCreator.Host
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Config.LoadConfig();

                // Start TopShelf
                var exitCode = HostFactory.Run(hostConfig =>
                {
                    hostConfig.Service<FakeStoreOrderService>(config =>
                    {
                        config.ConstructUsing(work => new FakeStoreOrderService());
                        config.WhenStarted((work, _) =>
                        {
                            Logger.Info("Starting application...");
                            work.Start();
                            return true;
                        });

                        config.WhenStopped((work, _) =>
                        {
                            work.Stop();
                            Logger.Info("Application terminated!");
                            return true;
                        });
                    });

                    hostConfig.RunAsLocalSystem();
                    hostConfig.SetServiceName("FakeStoreOrderCreator");
                    hostConfig.SetDisplayName("FakeStoreOrderCreator");
                    hostConfig.SetDescription("Creates JSON files that register the orders of the fake store.");

                });

                // For generic cases: (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
                int exitCodeValue = (int)exitCode;
                Environment.ExitCode = exitCodeValue;
                Console.WriteLine($"ExitCode: {Environment.ExitCode}");
            }
            catch (Exception ex)
            {
                // Terminates the service when errors are not caught by Topshelf at startup
                HandleStartupError(ex);
                Console.WriteLine($"{DateTime.Now} - Error: {ex}");
                Environment.Exit(1);
            }

        }

        private static void HandleStartupError(Exception exception)
        {
            // Creates a file due to the chance that the Logger may not have been initialized

            string fatalErrorDirectory = Path.Combine(AppContext.BaseDirectory, "StartupErrors");
            if (!Directory.Exists(fatalErrorDirectory))
                Directory.CreateDirectory(fatalErrorDirectory);

            string timeStamp = DateTime.Now.Date.ToString("yyyyMMdd");
            string file = Path.Combine(fatalErrorDirectory, $"{timeStamp}_ERROR_.txt");
            string errorMsg = $"{DateTime.Now} - Error during application startup: {exception.ToString()}{Environment.NewLine}";
            File.AppendAllText(file, errorMsg);
        }
    }
}
