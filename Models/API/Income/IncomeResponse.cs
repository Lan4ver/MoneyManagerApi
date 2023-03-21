namespace MoneyManagerApi.Models.API.Income
{
    public class IncomeResponse
    {
        public long IncomeId { get; set; }
        public long WalletId { get; set; }
        public string Name { get; set; }
        public decimal Sum { get; set; }
        public string Date { get; set; }
    }
}
