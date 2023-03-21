using System;
using System.Collections.Generic;

namespace MoneyManagerApi.Models.Db
{
    public partial class Cost
    {
        public long Id { get; set; }
        public long WalletId { get; set; }
        public long CostTypeId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Sum { get; set; }
        public DateTime Date { get; set; }

        public virtual CostType CostType { get; set; } = null!;
        public virtual Wallet Wallet { get; set; } = null!;
    }
}
