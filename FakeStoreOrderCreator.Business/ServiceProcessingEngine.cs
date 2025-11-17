using FakeStoreOrderCreator.Business.Interfaces;
using FakeStoreOrderCreator.Business.Logging;
using FakeStoreOrderCreator.Library.Models.Api;
using FakeStoreOrderCreator.Library.Models.Enums;
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
        private List<Order> _paymentConfirmedOrders = new();
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

        #region Methods
        public async Task ProcessOrdersAsync(CancellationToken cancellationToken)
        {
            try
            {
                Logger.Info("Obtaining data from API...");
                _carts = await _apiService.GetCartsAsync(cancellationToken);
                _products = await _apiService.GetProductsAsync(cancellationToken);
                _users = await _apiService.GetUsersAsync(cancellationToken);
                Logger.Info("All data Obtained!");

                Logger.Info("Creating product register files");
                _fileService.CreateProductRegisterFiles(_products);
                Logger.Info("All product register files created");

                Logger.Info("Creating orders...");
                CreateOrders();
                Logger.Info("All orders added to application memory");

                Logger.Info("Creating order files...");
                _fileService.CreateOrderFiles(_orders);
                Logger.Info("All order files created");

                Logger.Info("Checking order payments...");
                ConfirmPayment();
                Logger.Info("Payments checked");

                Logger.Info("Creating payment confirmed files...");
                _fileService.CreatePaymentConfirmedFiles(_paymentConfirmedOrders);
                Logger.Info("All payment confirmed files created");

                Logger.Info("Delivering orders...");
                await DeliverOrdersAsync(cancellationToken);
                Logger.Info("All orders delivered");

                _orders = new();
                _paymentConfirmedOrders = new();
            }
            catch (OperationCanceledException)
            {
                Logger.Debug(_className, "ProcessOrdersAsync", "Processing was canceled via token");
                throw;
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
                    var order = new Order()
                    {
                        OrderGuid = Guid.NewGuid().ToString(),
                        OrderDate = DateTime.Now,
                        PaymentStatus = PaymentStatus.Pending,
                        ShippingStatus = ShippingStatus.NotStarted
                    };

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
                    Logger.Debug(_className, "CreateOrders", $"Order: {order.OrderGuid} added to application memory");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "CreateOrders", $"Error: {ex.Message}");
                throw;
            }
        }
        public void ConfirmPayment()
        {
            try
            {
                foreach (var order in _orders)
                {
                    Random rand = new Random();
                    bool paymentConfirmed = rand.Next(2) == 1;

                    if (paymentConfirmed)
                    {
                        order.PaymentStatus = PaymentStatus.Confirmed;
                        order.PaymentDate = DateTime.Now;
                        _paymentConfirmedOrders.Add(order);
                        Logger.Debug(_className, "ConfirmPayment", $"Payment confirmed for order: {order.OrderGuid}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "ConfirmPayment", $"Error: {ex.Message}");
                throw;
            }
        }
        public async Task DeliverOrdersAsync(CancellationToken cancellationToken)
        {
            try
            {
                foreach (var order in _paymentConfirmedOrders)
                {
                    order.ShippingStatus = ShippingStatus.Shipped;
                    order.ShippedDate = DateTime.Now;
                }

                Logger.Info("Creating order shipped files...");
                _fileService.CreateOrderShippedFiles(_paymentConfirmedOrders);
                Logger.Info("All order shipped files created");

                await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);

                foreach (var order in _paymentConfirmedOrders)
                {
                    order.ShippingStatus = ShippingStatus.Delivered;
                    order.DeliveredDate = DateTime.Now;
                }

                Logger.Info("Creating order delivered files...");
                _fileService.CreateOrderDeliveredFiles(_paymentConfirmedOrders);
                Logger.Info("All order delivered files created");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "DeliverOrdersAsync", $"Error: {ex.Message}");
                throw;
            }
        }
        #endregion
    }
}
