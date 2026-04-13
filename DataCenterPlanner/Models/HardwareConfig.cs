namespace DataCenterPlanner.Models
{
    public class HardwareConfig
    {
        public int RackUnits { get; set; } = 47;
        public int SwitchUnits { get; set; } = 1;

        public int Server12kUnits { get; set; } = 7;
        public int Server5kUnits { get; set; } = 3;

        public int Server12kIops { get; set; } = 12000;
        public int Server5kIops { get; set; } = 5000;

        public bool Allow12kServers { get; set; } = true;
        public bool Allow5kServers { get; set; } = true;
    }
}