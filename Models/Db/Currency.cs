using System;
using System.Collections.Generic;

namespace MoneyManagerApi.Models.Db
{
    public partial class Currency
    {
        public Currency()
        {
            Wallets = new HashSet<Wallet>();
        }

        public long Id { get; set; }
        public string Symbol { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
