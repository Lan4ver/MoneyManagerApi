using System.ComponentModel.DataAnnotations;

namespace MoneyManagerApi.Models.API.CostType
{
    public class CostTypeResponse
    {
        public long CostTypeId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }
}
