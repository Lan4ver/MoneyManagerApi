namespace MoneyManagerApi.Models.API.Income
{
    public class IncomeUpdateRequest
    {
        public long IncomeId { get; set; }
        public string Name { get; set; }
        public decimal Sum { get; set; }
        public DateTime Date { get; set; }
    }
}
