using System;
using System.Collections.Generic;

namespace MoneyManagerApi.Models.Db
{
    public partial class Wallet
    {
        public Wallet()
        {
            Costs = new HashSet<Cost>();
            Incomes = new HashSet<Income>();
        }

        public long Id { get; set; }
        public long UserId { get; set; }
        public long CurrencyId { get; set; }
        public string Name { get; set; } = null!;

        public virtual Currency Currency { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Cost> Costs { get; set; }
        public virtual ICollection<Income> Incomes { get; set; }
    }
}
