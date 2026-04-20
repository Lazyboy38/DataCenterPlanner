using System.Collections.Generic;

namespace DataCenterPlanner.Models
{
    public class ShoppingSummary
    {
        public decimal ComputeCost { get; set; }
        public decimal InfrastructureCost { get; set; }
        public decimal ModuleCost { get; set; }

        public List<ShoppingCategory> Categories { get; set; } = new();

        public decimal TotalCost =>
            ComputeCost + InfrastructureCost + ModuleCost;
    }
}