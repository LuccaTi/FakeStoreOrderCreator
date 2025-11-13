using FakeStoreOrderCreator.Business.Interfaces;
using FakeStoreOrderCreator.Business.Logging;
using FakeStoreOrderCreator.Library.Models.Api;
using FakeStoreOrderCreator.Library.Models.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreOrderCreator.Business
{
    public class ServiceProcessingEngine : IServiceProcessingEngine
    {
        #region Attributes
        private const string _className = "ServiceProcessingEngine";
        private List<Cart> _carts = new();
        private List<Product> _products = new();
        private List<User> _users = new();
        private List<Order> _orders = new();
        #endregion

        #region Dependencies
        private readonly IApiService _apiService;
        private readonly IFileService _fileService;
        #endregion

        public ServiceProcessingEngine(IApiService apiService, IFileService fileService)
        {
            try
            {
                _apiService = apiService;
                _fileService = fileService;
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "ServiceProcessingEngine Constructor", $"Error: {ex.Message}");
                throw;
            }
        }

        public async Task ProcessOrdersAsync()
        {
            try
            {
                Logger.Info("Obtaining data from API...");
                _carts = await _apiService.GetCartsAsync();
                _products = await _apiService.GetProductsAsync();
                _users = await _apiService.GetUsersAsync();
                Logger.Info("All data Obtained!");

                Logger.Info("Creating Orders...");
                CreateOrders();
                _orders = new();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "ProcessOrdersAsync", $"Error: {ex.Message}");
                throw;
            }
        }

        public void CreateOrders()
        {
            try
            {
                foreach (var cart in _carts)
                {
                    Logger.Debug(_className, "CreateOrders", $"Cart id: {cart.Id}");
                    var order = new Order();
                    var user = _users.FirstOrDefault(u => u.Id == cart.UserId);

                    if (user == null)
                    {
                        Logger.Debug(_className, "CreateOrders", $"User not found for cart with id: {cart.Id}, order will not be created!");
                        continue;
                    }
                    else
                    {
                        order.Customer = new Customer()
                        {
                            Address = user.Address,
                            Email = user.Email,
                            Username = user.Username,
                            Password = user.Password,
                            Name = user.Name,
                            Phone = user.Phone
                        };
                        Logger.Debug(_className, "CreateOrders", $"Customer: {order.Customer.Name?.FirstName} {order.Customer.Name?.LastName}");
                    }

                    try
                    {
                        foreach (var cartProduct in cart.Products!)
                        {
                            var product = _products.FirstOrDefault(p => p.Id == cartProduct.ProductId);
                            if (product == null)
                                throw new Exception($"Cart product with id: {cartProduct.ProductId} was not found in registered products list, order will not be created!");

                            var orderItem = new OrderItem()
                            {
                                Title = product.Title,
                                Price = product.Price,
                                Description = product.Description,
                                Category = product.Category,
                                Image = product.Image,
                                Quantity = cartProduct.Quantity
                            };

                            order.Items.Add(orderItem);
                            Logger.Debug(_className, "CreateOrders", $"Product: {orderItem.Title} added - Quantity: {orderItem.Quantity}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug(_className, "CreateOrders", $"{ex.Message}");
                        continue;
                    }


                    _orders.Add(order);
                    Logger.Debug(_className, "CreateOrders", $"Order added to application memory");
                }

                Logger.Debug(_className, "CreateOrders", "All orders added to application memory, proceeding to create order files...");

                _fileService.CreateOrderFiles(_orders);
                Logger.Info("All order files created");
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "CreateOrders", $"Error: {ex.Message}");
                throw;
            }
        }
    }
}
