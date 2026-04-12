namespace DataCenterPlanner.Models
{
    public class CategoryRequest
    {
        public ServerCategory Category { get; set; }
        public int TargetIops { get; set; }
    }
}