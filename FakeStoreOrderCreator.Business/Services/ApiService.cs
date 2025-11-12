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
        public List<Cart> GetCarts()
        {
            try
            {
                Logger.Debug(_className, "GetCarts", $"Connecting to endpoint: {_httpClient.BaseAddress + "Carts"}");
                var response = _httpClient.GetAsync("Cart").Result;
                response.EnsureSuccessStatusCode();
                Logger.Debug(_className, "GetCarts", $"Connection Success!");
                var carts = response.Content.ReadFromJsonAsync<List<Cart>>().Result;
                return carts ?? new List<Cart>();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "GetCarts", $"Error: {ex.Message}");
                throw;
            }
        }

        public List<Product> GetProducts()
        {
            try
            {
                Logger.Debug(_className, "GetProducts", $"Connecting to endpoint: {_httpClient.BaseAddress + "Products"}");
                var response = _httpClient.GetAsync("Product").Result;
                response.EnsureSuccessStatusCode();
                Logger.Debug(_className, "GetProducts", $"Connection Success!");
                var products = response.Content.ReadFromJsonAsync<List<Product>>().Result;
                return products ?? new List<Product>();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "GetProducts", $"Error: {ex.Message}");
                throw;
            }
        }

        public List<User> GetUsers()
        {
            try
            {
                Logger.Debug(_className, "GetUsers", $"Connecting to endpoint: {_httpClient.BaseAddress + "Users"}");
                var response = _httpClient.GetAsync("User").Result;
                response.EnsureSuccessStatusCode();
                Logger.Debug(_className, "GetUsers", $"Connection Success!");
                var users = response.Content.ReadFromJsonAsync<List<User>>().Result;
                return users ?? new List<User>();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "GetUsers", $"Error: {ex.Message}");
                throw;
            }
        }
    }
}
