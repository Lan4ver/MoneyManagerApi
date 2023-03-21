namespace MoneyManagerApi.Models.API.Wallet
{
    public class WalletDetailsResponse
    {
        public long WalletId { get; set; }
        public string Name { get; set; }
        public decimal Income { get; set; }
        public decimal Outcome { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
    }
}
