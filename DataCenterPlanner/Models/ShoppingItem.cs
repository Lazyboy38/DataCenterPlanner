namespace DataCenterPlanner.Models
{
    public class ShoppingItem
    {
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;
    }
}