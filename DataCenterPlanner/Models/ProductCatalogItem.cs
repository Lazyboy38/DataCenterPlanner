namespace DataCenterPlanner.Models
{
    public class ProductCatalogItem
    {
        public string Id { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public ProductCategory Category { get; set; }

        public decimal UnitPrice { get; set; }

        public int PackSize { get; set; } = 1;

        public int RackUnits { get; set; }

        public int Iops { get; set; }

        public int SpeedGbps { get; set; }

        public bool IsCalculated { get; set; } = false;
    }
}