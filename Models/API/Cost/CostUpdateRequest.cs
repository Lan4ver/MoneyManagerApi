namespace MoneyManagerApi.Models.API.Cost
{
    public class CostUpdateRequest
    {
        public long CostId { get; set; }

        public string Name { get; set; }

        public decimal Sum { get; set; }

        public DateTime Date { get; set; }

        public long WalletId { get; set; }

        public long CostTypeId { get; set; }
    }
}
