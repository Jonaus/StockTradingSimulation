using Newtonsoft.Json;
using RestSharp;
using System;

namespace StockTradingSimulationAPI.Tests
{
    class APIClient
    {
        private static readonly Uri BaseUri = new Uri(@"http://localhost:64328");
        public static RestClient Client { get; } = new RestClient(BaseUri);

        public static T Get<T>(string token, string endpoint)
        {
            return Execute<T>(token, endpoint, Method.GET);
        }

        public static T Post<T>(string token, string endpoint, object o = null)
        {
            return Execute<T>(token, endpoint, Method.POST, o);
        }

        public static T Put<T>(string token, string endpoint, object o = null)
        {
            return Execute<T>(token, endpoint, Method.PUT, o);
        }

        public static T Delete<T>(string token, string endpoint)
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
    }
}
