using RestSharp;
using StockTradingSimulationWebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace StockTradingSimulationWebClient.Core
{
    public static class ApiClient
    {
        private static readonly Uri BaseUri = new Uri(@"http://localhost:64328");
        public static RestClient Client { get; private set; } = new RestClient(BaseUri);
        public static int Port => Client.BaseUrl.Port;

        public static void ChangePort(int newPort)
        {
            var builder = new UriBuilder(BaseUri) {Port = newPort};
            Client = new RestClient(builder.Uri);
        }

        public static IEnumerable<Stock> GetStocks(string token)
        {
            return Get<List<Stock>>(token, "api/stocks");
        }

        public static IEnumerable<User> GetUsers(string token)
        {
            return Get<List<User>>(token, "api/users");
        }

        public static IEnumerable<MoneyHistory> GetHistory(string token)
        {
            return Get<List<MoneyHistory>>(token, "api/users/self/history");
        }

        public static User GetUser(string token, string userId)
        {
            return Get<User>(token, $"api/Users/{userId}");
        }

        public static Stock GetStock(string token, int stockId)
        {
            return Get<Stock>(token, $"api/Stocks/{stockId}");
        }

        public static void AddStock(string token, Stock stock)
        {
            Post<string>(token, "api/Stocks", new
            {
                Id = 0,
                Fullname = stock.Fullname,
                Ticker=  stock.Ticker
            });
        }

        public static void EditStock(string token, Stock stock)
        {
            Put<string>(token, $"api/Stocks/{stock.Id}", new
            {
                fullname = stock.Fullname,
                ticker = stock.Ticker
            });
        }

        public static void DeleteStock(string token, int stockId)
        {
            Delete<string>(token, $"api/Stocks/{stockId}");
        }

        public static void DeleteUser(string token, string userId)
        {
            Delete<string>(token, $"api/Users/{userId}");
        }

        public static User GetSelf(string token)
        {
            return Get<User>(token, "api/Users/self");
        }

        public static float GetSelfBalance(string token)
        {
            return Get<float>(token, "api/Users/self/balance");
        }

        public static float GetSelfRealBalance(string token)
        {
            return Get<float>(token, "api/Users/self/realbalance");
        }

        public static float GetBalance(string token, string id)
        {
            return Get<float>(token, $"api/Users/{id}/balance");
        }

        public static float GetRealBalance(string token, string id)
        {
            return Get<float>(token, $"api/Users/{id}/realbalance");
        }

        public static IEnumerable<Position> GetSelfPositions(string token)
        {
            return Get<List<Position>>(token, "api/positions");
        }

        public static float GetStockPrice(string token, int id)
        {
            return Get<float>(token, $"api/stocks/{id}/price");
        }

        public static void ClosePosition(string token, int id)
        {
            Post<string>(token, $"api/positions/{id}/close");
        }

        public static void OpenPosition(string token, NewPositionViewModel model)
        {
            Post<string>(token, $"api/positions", new
            {
                StockId = model.SelectedStockId,
                TransactionType = model.SelectedTransactionId,
                Quantity = model.Quantity,
                // Stoploss = 0
            });
        }

        public static void SendMoney(string token, TransactionViewModel model)
        {
            Post<string>(token, $"api/users/{model.UserId}/balance", new
            {
                Amount = model.Amount
            });
        }

        #region Private

        private static T Get<T>(string token, string endpoint)
        {
            return Execute<T>(token, endpoint, Method.GET);
        }

        private static T Post<T>(string token, string endpoint, object o = null)
        {
            return Execute<T>(token, endpoint, Method.POST, o);
        }

        private static T Put<T>(string token, string endpoint, object o = null)
        {
            return Execute<T>(token, endpoint, Method.PUT, o);
        }

        private static T Delete<T>(string token, string endpoint)
        {
            return Execute<T>(token, endpoint, Method.DELETE);
        }

        private static T Execute<T>(string token, string endpoint, Method method, object o = null)
        {
            var request = new RestRequest(endpoint, method);
            request.AddParameter("Authorization", $"Bearer {token}", ParameterType.HttpHeader);
            if (o != null)
            {
                request.AddHeader("Content-type", "application/json");
                request.AddJsonBody(o);
            }

            IRestResponse response = Client.Execute(request);
            try
            {
                return JsonConvert.DeserializeObject<T>(response.Content, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
            }
            catch
            {
                return default(T);
            }
        }

        #endregion
    }
}