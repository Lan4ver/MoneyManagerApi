using System.ComponentModel.DataAnnotations;

namespace MoneyManagerApi.Models.API.Income
{
    public class IncomeRequest
    {
        public int WalletId { get; set; }
        public string Name { get; set; }
        public decimal Sum { get; set; }
        public DateTime Date { get; set; }
    }
}
