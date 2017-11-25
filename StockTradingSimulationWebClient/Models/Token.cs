using System.Diagnostics.CodeAnalysis;

namespace StockTradingSimulationWebClient.Models
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Token
    {
        public string access_token { get; set; }
        public string token_bearer { get; set; }
        public int expires_in { get; set; }
    }
}