using System.Collections.Generic;

namespace DataCenterPlanner.Models
{
    public class CategoryResultCard
    {
        public ServerCategory Category { get; set; }

        public int RackCount { get; set; }
        public int Total12kServers { get; set; }
        public int Total5kServers { get; set; }
        public int TotalSwitches { get; set; }
        public int PlannedIops { get; set; }

        public string AccentColorHex =>
            Category switch
            {
                ServerCategory.SystemX => "#F2C94C",
                ServerCategory.RISC => "#4AAEFF",
                ServerCategory.Mainframe => "#BB86FC",
                ServerCategory.GPU => "#57D68D",
                _ => "#2FA8FF"
            };

        public List<RackPlan> Racks { get; set; } = new();
    }
}