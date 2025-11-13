using FakeStoreOrderCreator.Business.Interfaces;
using FakeStoreOrderCreator.Business.Logging;
using FakeStoreOrderCreator.Library.Models.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeStoreOrderCreator.Business.Configuration;
using System.Text.Json;

namespace FakeStoreOrderCreator.Business.Services
{
    public class FileService : IFileService
    {
        #region Attributes
        private const string _className = "FileService";
        private string _ordersDirectory = Config.FakeStoreDirectory + "\\OrdersToProcess";
        #endregion
        public void CreateOrderFiles(List<Order> orders)
        {
            try
            {
                string basePath = Path.Combine(_ordersDirectory, DateTime.Now.ToString("yyyy/MM/dd")).Replace(@"/", "\\");
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                    Logger.Debug(_className, "CreateOrderFiles", $"Directory created: {basePath}");
                }

                foreach (var order in orders)
                {
                    string orderFileName = $"{DateTime.Now.Ticks}_order_created.json";
                    string orderFilePath = Path.Combine(basePath, orderFileName);

                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    string orderFileContent = JsonSerializer.Serialize(order, options);

                    try
                    {
                        File.WriteAllText(orderFilePath, orderFileContent);
                        Logger.Debug(_className, "CreateOrderFiles", $"File created: {orderFilePath}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(_className, "CreateOrderFiles", $"Error while creating file: {orderFilePath} - {ex.Message}");
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "CreateOrderFiles", $"Error: {ex.Message}");
                throw;
            }
        }
    }
}
