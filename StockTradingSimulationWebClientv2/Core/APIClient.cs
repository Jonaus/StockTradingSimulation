using RestSharp;
using System;
using StockTradingSimulationWebClientv2.Models;

namespace StockTradingSimulationWebClientv2.Core
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
    }
}