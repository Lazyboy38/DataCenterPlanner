namespace DataCenterPlanner.Models
{
    public class UserSettings
    {
        public int PlanningModeIndex { get; set; } = 0;
        public bool Redundancy { get; set; } = false;
        public bool Allow12kServers { get; set; } = true;
        public bool Allow5kServers { get; set; } = true;
        public string SystemXIops { get; set; } = "100000";
        public string RiscIops { get; set; } = "50000";
        public string MainframeIops { get; set; } = "50000";
        public string GpuIops { get; set; } = "40000";
    }
}
