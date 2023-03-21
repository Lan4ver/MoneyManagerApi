using System;
using System.Collections.Generic;

namespace MoneyManagerApi.Models.Db
{
    public partial class CostType
    {
        public CostType()
        {
            Costs = new HashSet<Cost>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Image { get; set; } = null!;
        public string Color { get; set; } = null!;

        public virtual ICollection<Cost> Costs { get; set; }
    }
}
