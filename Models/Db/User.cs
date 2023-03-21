using System;
using System.Collections.Generic;

namespace MoneyManagerApi.Models.Db
{
    public partial class User
    {
        public User()
        {
            Wallets = new HashSet<Wallet>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;

        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
