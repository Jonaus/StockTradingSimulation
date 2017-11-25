using System.Diagnostics.CodeAnalysis;

namespace StockTradingSimulationAPI.ViewModels
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class TokenViewModel
    {
        public string access_token { get; set; }
        public string token_bearer { get; set; }
        public int expires_in { get; set; }
    }
}