using FakeStoreOrderCreator.Business;
using FakeStoreOrderCreator.Business.Configuration;
using FakeStoreOrderCreator.Business.Interfaces;
using FakeStoreOrderCreator.Business.Logging;
using FakeStoreOrderCreator.Business.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;
using System.Net.Security;
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

                // DI Container
                var services = new ServiceCollection();
                ConfigureServices(services);
                var serviceProvider = services.BuildServiceProvider();

                // Start TopShelf
                var exitCode = HostFactory.Run(hostConfig =>
                {
                    hostConfig.Service<ServiceLifeCycleManager>(config =>
                    {
                        config.ConstructUsing(work => serviceProvider.GetRequiredService<ServiceLifeCycleManager>());
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

        private static void ConfigureServices(IServiceCollection services)
        {
            try
            {
                // HttpClient
                bool isDevelopment = IsDevelopmentEnvironment();
                services.AddHttpClient<IApiService, ApiService>(client =>
                {
                    client.BaseAddress = new Uri(Config.ApiUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new System.Net.Http.HttpClientHandler();

                    if (isDevelopment)
                    {
                        handler.ServerCertificateCustomValidationCallback =
                            (message, cert, chain, errors) => true;
                        Logger.Info("SSL certificate validation is disabled (Development mode)");
                    }

                    return handler;
                });

                services.AddSingleton<IServiceProcessingOrchestrator, ServiceProcessingOrchestrator>();
                services.AddSingleton<IServiceProcessingEngine, ServiceProcessingEngine>();
                services.AddSingleton<IFileService, FileService>();
                services.AddSingleton<ServiceLifeCycleManager>();
            }
            catch (Exception ex)
            {
                Logger.Error("Program.cs", "ConfigureServices", $"Error while configuring services: {ex.Message}");
                throw;
            }
        }
        private static bool IsDevelopmentEnvironment()
        {
            try
            {
                string apiUrl = Config.ApiUrl.ToLower();
                return apiUrl.Contains("localhost") || apiUrl.Contains("127.0.0.1");
            }
            catch (Exception ex)
            {
                Logger.Error("Program.cs", "IsDevelopmentEnvironment", $"Error while trying to get application environment: {ex.Message}");
                throw;
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
