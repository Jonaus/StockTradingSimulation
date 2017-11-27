using System;
using System.Collections;
using System.Collections.Generic;
using RestSharp;
using StockTradingSimulationWebClient.Models;

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

        public static User GetSelf(string token)
        {
            var request = new RestRequest("api/Users/self", Method.GET);
            request.AddParameter("Authorization", $"Bearer {token}", ParameterType.HttpHeader);

            IRestResponse<User> response = Client.Execute<User>(request);
            return response.Data;
        }

        public static float GetSelfBalance(string token)
        {
            var request = new RestRequest("api/Users/self/balance", Method.GET);
            request.AddParameter("Authorization", $"Bearer {token}", ParameterType.HttpHeader);

            IRestResponse<float> response = Client.Execute<float>(request);
            return response.Data;
        }

        public static IEnumerable<Position> GetSelfPositions(string token)
        {
            var request = new RestRequest("api/positions", Method.GET);
            request.AddParameter("Authorization", $"Bearer {token}", ParameterType.HttpHeader);

            IRestResponse<List<Position>> response = Client.Execute<List<Position>>(request);
            return response.Data;
        }

        public static float GetStockPrice(string token, int id)
        {
            var request = new RestRequest($"api/stocks/{id}/price", Method.GET);
            request.AddParameter("Authorization", $"Bearer {token}", ParameterType.HttpHeader);

            IRestResponse<float> response = Client.Execute<float>(request);
            return response.Data;
        }
    }
}