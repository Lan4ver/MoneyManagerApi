using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MoneyManagerApi.Models.API.Cost
{
    public class CostRequest
    {
        public string Name { get; set; }

        public decimal Sum { get; set; }

        public DateTime Date { get; set; }

        public long WalletId { get; set; }

        public long CostTypeId { get; set; }
    }
}
