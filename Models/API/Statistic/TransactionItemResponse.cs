namespace MoneyManagerApi.Models.API.Statistic
{
    public class TransactionItemResponse
    {
        public long TransactionId { get; set; }
        public string Name { get; set; }
        public string DateTime { get; set; }
        public decimal Amount { get; set; }
        public string TypeImage { get; set; }
        public string TransactionType { get; set; }
    }
}
