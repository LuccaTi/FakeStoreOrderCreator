using FakeStoreOrderCreator.Business.Interfaces;
using FakeStoreOrderCreator.Business.Logging;
using FakeStoreOrderCreator.Library.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FakeStoreOrderCreator.Business.Services
{
    public class ApiService : IApiService
    {
        #region Attributes
        private const string _className = "ApiService";
        #endregion

        #region Dependencies
        private readonly HttpClient _httpClient;
        #endregion

        public ApiService(HttpClient httpClient)
        {
            try
            {
                _httpClient = httpClient;
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "ApiService Constructor", $"Error: {ex.Message}");
                throw;
            }
        }

        #region Methods
        public async Task<List<Cart>> GetCartsAsync(CancellationToken cancellationToken)
        {
            try
            {
                Logger.Debug(_className, "GetCartsAsync", $"Connecting to endpoint: {_httpClient.BaseAddress + "Carts"}");
                var response = _httpClient.GetAsync("Carts").Result;
                response.EnsureSuccessStatusCode();
                Logger.Debug(_className, "GetCartsAsync", $"Connection Success!");
                var carts = await response.Content.ReadFromJsonAsync<List<Cart>>();

                if (carts is null || carts.Count == 0)
                {
                    throw new Exception($"Carts list obtained was null or empty, application can't create orders, status code: {response.StatusCode}");
                }

                return carts;
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "GetCartsAsync", $"Error while consuming API to obtain data: {ex.Message}");
                throw;
            }
        }
        public async Task<List<Product>> GetProductsAsync(CancellationToken cancellationToken)
        {
            try
            {
                Logger.Debug(_className, "GetProductsAsync", $"Connecting to endpoint: {_httpClient.BaseAddress + "Products"}");
                var response = _httpClient.GetAsync("Products").Result;
                response.EnsureSuccessStatusCode();
                Logger.Debug(_className, "GetProductsAsync", $"Connection Success!");
                var products = await response.Content.ReadFromJsonAsync<List<Product>>();

                if (products is null || products.Count == 0)
                {
                    throw new Exception($"Products list obtained was null or empty, application can't create orders, status code: {response.StatusCode}");
                }

                return products;
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "GetProductsAsync", $"Error while consuming API to obtain data: {ex.Message}");
                throw;
            }
        }
        public async Task<List<User>> GetUsersAsync(CancellationToken cancellationToken)
        {
            try
            {
                Logger.Debug(_className, "GetUsersAsync", $"Connecting to endpoint: {_httpClient.BaseAddress + "Users"}");
                var response = _httpClient.GetAsync("Users").Result;
                response.EnsureSuccessStatusCode();
                Logger.Debug(_className, "GetUsersAsync", $"Connection Success!");
                var users = await response.Content.ReadFromJsonAsync<List<User>>();

                if (users is null || users.Count == 0)
                {
                    throw new Exception($"Users list obtained was null or empty, application can't create orders, status code: {response.StatusCode}");
                }

                return users;
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "GetUsersAsync", $"Error while consuming API to obtain data: {ex.Message}");
                throw;
            }
        }

        #endregion
    }
}
