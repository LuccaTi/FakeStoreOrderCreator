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
using FakeStoreOrderCreator.Library.Models.Api;
using System.Text.Json.Nodes;
using System.Reflection;
using System.Text.Json.Serialization;

namespace FakeStoreOrderCreator.Business.Services
{
    public class FileService : IFileService
    {
        #region Attributes
        private const string _className = "FileService";
        private string _ordersDirectory = Config.FakeStoreDirectory + "\\Orders";
        #endregion

        #region Methods
        public void CreateProductRegisterFiles(List<Product> products)
        {
            try
            {
                string basePath = Path.Combine(_ordersDirectory, DateTime.Now.ToString("yyyy/MM/dd")).Replace(@"/", "\\");
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                    Logger.Debug(_className, "CreateOrderFiles", $"Directory created: {basePath}");
                }
                foreach (var item in products)
                {
                    string fileName = $"{DateTime.Now.Ticks}_product_stock_register.json";
                    string filePath = Path.Combine(basePath, fileName);

                    try
                    {
                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true
                        };

                        var productNode = JsonSerializer.SerializeToNode(item);
                        var productJsonObject = productNode!.AsObject();
                        productJsonObject.Remove("Id");
                        productJsonObject.Add("QuantityToRegister", 100);

                        string productFileContent = productJsonObject.ToJsonString(options);
                        File.WriteAllText(filePath, productFileContent);
                        Logger.Debug(_className, "CreateProductRegisterFiles", $"File created: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(_className, "CreateProductRegisterFiles", $"Error while creating file: {filePath} - {ex.Message}");
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "CreateProductRegisterFiles", $"Error: {ex.Message}");
                throw;
            }
        }
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

                for (int i = orders.Count - 1; i >= 0; i--)
                {
                    var item = orders[i];
                    string fileName = $"{DateTime.Now.Ticks}_{item.OrderGuid}_order_created.json";
                    string filePath = Path.Combine(basePath, fileName);
                    try
                    {

                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            Converters = { new JsonStringEnumConverter() }
                        };
                        string fileContent = JsonSerializer.Serialize(item, options);

                        File.WriteAllText(filePath, fileContent);
                        Logger.Debug(_className, "CreateOrderFiles", $"File created: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(_className, "CreateOrderFiles", $"Error while creating file: {filePath} - {ex.Message}");
                        Logger.Debug(_className, "CreateOrderFiles", $"Files with guid: {item.OrderGuid} will be deleted");
                        DeleteOrderFiles(item.OrderGuid!, basePath);
                        orders.RemoveAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "CreateOrderFiles", $"Error: {ex.Message}");
                throw;
            }
        }
        public void CreatePaymentConfirmedFiles(List<Order> paymentConfirmedOrders)
        {
            try
            {
                string basePath = Path.Combine(_ordersDirectory, DateTime.Now.ToString("yyyy/MM/dd")).Replace(@"/", "\\");
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                    Logger.Debug(_className, "CreatePaymentConfirmedFiles", $"Directory created: {basePath}");
                }

                for (int i = paymentConfirmedOrders.Count - 1; i >= 0; i--)
                {
                    var item = paymentConfirmedOrders[i];
                    string fileName = $"{DateTime.Now.Ticks}_{item.OrderGuid}_order_payment_confirmed.json";
                    string filePath = Path.Combine(basePath, fileName);
                    try
                    {
                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            Converters = { new JsonStringEnumConverter() }
                        };
                        var jsonObject = new JsonObject();
                        jsonObject.Add("OrderGuid", item.OrderGuid);
                        jsonObject.Add("PaymentStatus", item.PaymentStatus.ToString());
                        jsonObject.Add("PaymentDate", item.PaymentDate);
                        jsonObject.Add("TotalPrice", item.TotalPrice);

                        string fileContent = jsonObject.ToJsonString(options);
                        File.WriteAllText(filePath, fileContent);
                        Logger.Debug(_className, "CreatePaymentConfirmedFiles", $"File created: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(_className, "CreatePaymentConfirmedFiles", $"Error while creating file: {filePath} - {ex.Message}");
                        Logger.Debug(_className, "CreatePaymentConfirmedFiles", $"Files with guid: {item.OrderGuid} will be deleted");
                        DeleteOrderFiles(item.OrderGuid!, basePath);
                        paymentConfirmedOrders.RemoveAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "CreatePaymentConfirmedFiles", $"Error: {ex.Message}");
                throw;
            }
        }
        public void CreateOrderShippedFiles(List<Order> paymentConfirmedOrders)
        {
            try
            {
                string basePath = Path.Combine(_ordersDirectory, DateTime.Now.ToString("yyyy/MM/dd")).Replace(@"/", "\\");
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                    Logger.Debug(_className, "CreateOrderShippedFiles", $"Directory created: {basePath}");
                }

                for (int i = paymentConfirmedOrders.Count - 1; i >= 0; i--)
                {
                    var item = paymentConfirmedOrders[i];
                    string fileName = $"{DateTime.Now.Ticks}_{item.OrderGuid}_order_shipped.json";
                    string filePath = Path.Combine(basePath, fileName);
                    try
                    {
                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            Converters = { new JsonStringEnumConverter() }
                        };
                        var jsonObject = new JsonObject();
                        jsonObject.Add("OrderGuid", item.OrderGuid);
                        jsonObject.Add("ShippingStatus", item.ShippingStatus.ToString());
                        jsonObject.Add("ShippedDate", item.ShippedDate);

                        string fileContent = jsonObject.ToJsonString(options);

                        File.WriteAllText(filePath, fileContent);
                        Logger.Debug(_className, "CreateOrderShippedFiles", $"File created: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(_className, "CreateOrderShippedFiles", $"Error while creating file: {filePath} - {ex.Message}");
                        Logger.Debug(_className, "CreateOrderShippedFiles", $"Files with guid: {item.OrderGuid} will be deleted");
                        DeleteOrderFiles(item.OrderGuid!, basePath);
                        paymentConfirmedOrders.RemoveAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "CreateOrderShippedFiles", $"Error: {ex.Message}");
                throw;
            }
        }
        public void CreateOrderDeliveredFiles(List<Order> paymentConfirmedOrders)
        {
            try
            {
                string basePath = Path.Combine(_ordersDirectory, DateTime.Now.ToString("yyyy/MM/dd")).Replace(@"/", "\\");
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                    Logger.Debug(_className, "CreateOrderDeliveredFiles", $"Directory created: {basePath}");
                }

                for (int i = paymentConfirmedOrders.Count - 1; i >= 0; i--)
                {
                    var item = paymentConfirmedOrders[i];
                    string fileName = $"{DateTime.Now.Ticks}_{item.OrderGuid}_order_delivered.json";
                    string filePath = Path.Combine(basePath, fileName);
                    try
                    {
                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            Converters = { new JsonStringEnumConverter() }
                        };
                        var jsonObject = new JsonObject();
                        jsonObject.Add("OrderGuid", item.OrderGuid);
                        jsonObject.Add("ShippingStatus", item.ShippingStatus.ToString());
                        jsonObject.Add("DeliveredDate", item.DeliveredDate);

                        string fileContent = jsonObject.ToJsonString(options);

                        File.WriteAllText(filePath, fileContent);
                        Logger.Debug(_className, "CreateOrderDeliveredFiles", $"File created: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(_className, "CreateOrderDeliveredFiles", $"Error while creating file: {filePath} - {ex.Message}");
                        Logger.Debug(_className, "CreateOrderDeliveredFiles", $"Files with guid: {item.OrderGuid} will be deleted");
                        DeleteOrderFiles(item.OrderGuid!, basePath);
                        paymentConfirmedOrders.RemoveAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "CreateOrderDeliveredFiles", $"Error: {ex.Message}");
                throw;
            }
        }
        public void DeleteOrderFiles(string orderGuid, string basePath)
        {
            try
            {
                var orderFiles = Directory.GetFiles(basePath).Where(file => Path.GetFileName(file).Contains(orderGuid)).ToList();
                foreach (var file in orderFiles)
                {
                    try
                    {
                        Logger.Debug(_className, "DeleteOrderFiles", $"Deleting file: {file}");
                        File.Delete(file);
                        Logger.Debug(_className, "DeleteOrderFiles", $"File deleted!");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(_className, "DeleteOrderFiles", $"Error while trying to delete file: {file} - {ex.Message}");
                        try
                        {
                            Logger.Debug(_className, "DeleteOrderFiles", $"Moving file to invalid files directory...");
                            string invalidFilesPath = _ordersDirectory + "\\InvalidOrders\\" + DateTime.Now.ToString("yyyy/MM/dd").Replace(@"/", "\\");
                            if (!Directory.Exists(invalidFilesPath))
                            {
                                Directory.CreateDirectory(invalidFilesPath);
                            }

                            string destFile = Path.Combine(invalidFilesPath, Path.GetFileName(file));
                            Logger.Debug(_className, "DeleteOrderFiles", $"Moving file to invalid files directory...");
                            File.Move(file, destFile);
                            Logger.Debug(_className, "DeleteOrderFiles", $"File moved: {destFile}");
                        }
                        catch (Exception moveEx)
                        {
                            Logger.Error(_className, "DeleteOrderFiles", $"CRITICAL: Could not delete or move file: {file}. Manual intervention is required, pleace check. Move Error: {moveEx.Message}");
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "DeleteOrderFiles", $"Error: {ex.Message}");
                throw;
            }
        }
        #endregion
    }
}
