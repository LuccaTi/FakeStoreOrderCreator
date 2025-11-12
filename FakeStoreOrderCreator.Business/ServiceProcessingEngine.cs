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

        public void ProcessOrders()
        {
            try
            {
                Logger.Info("Obtaining data from API...");
                _carts = _apiService.GetCarts();
                _products = _apiService.GetProducts();
                _users = _apiService.GetUsers();
                Logger.Info("Data Obtained!");
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "ProcessOrders", $"Error: {ex.Message}");
                throw;
            }
        }
    }
}
