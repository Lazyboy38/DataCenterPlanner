using System.Collections.Generic;
using System.Linq;

namespace DataCenterPlanner.Models
{
    public class ShoppingCategory
    {
        public string Name { get; set; } = "";
        public string AccentColorHex { get; set; } = "#2FA8FF";

        public List<ShoppingItem> Items { get; set; } = new();

        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
    }
}