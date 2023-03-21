using System;
using System.Collections.Generic;

namespace MoneyManagerApi.Models.Db
{
    public partial class Income
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Sum { get; set; }
        public DateTime Date { get; set; }
        public long WalletId { get; set; }

        public virtual Wallet Wallet { get; set; } = null!;
    }
}
