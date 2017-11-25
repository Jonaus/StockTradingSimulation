using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace StockTradingSimulationAPI.Helpers
{
    public static class StockHelper
    {
        public static async Task<float> GetStockPrice(string ticker)
        {
            var url = $"https://query2.finance.yahoo.com/v10/finance/quoteSummary/{ticker}?modules=financialData";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    Task<HttpResponseMessage> getResponse = httpClient.GetAsync(url);

                    HttpResponseMessage response = await getResponse;
                    var responseJsonString = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseJsonString);
                    return float.Parse(json["quoteSummary"]["result"][0]["financialData"]["currentPrice"]["raw"]
                        .ToString());
                }
                catch
                {
                    return -1;
                }
            }
        }
    }
}
