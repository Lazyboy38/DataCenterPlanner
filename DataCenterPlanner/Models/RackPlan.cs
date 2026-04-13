namespace DataCenterPlanner.Models
{
    public class RackPlan
    {
        private const double MaxBarWidth = 220.0;
        private const int RackCapacityUnits = 47;

        public int RackNumber { get; set; }

        public ServerCategory Category { get; set; }

        public int Count12kServers { get; set; }
        public int Count5kServers { get; set; }

        // Basis aus alter Logik bleibt erstmal erhalten
        public int SwitchCount { get; set; } = 1;

        public bool IsMainRack { get; set; }

        // Neue Netzwerk-bezogene Werte
        public int RequiredRackSwitches { get; set; } = 1;
        public int ExtraMainSwitches { get; set; } = 0;

        public int TotalDisplayedSwitches => RequiredRackSwitches + ExtraMainSwitches;

        public string MainRackLabel => IsMainRack ? " [GLOBAL MAIN RACK]" : "";

        public int UsedUnits =>
            (Count12kServers * 7) +
            (Count5kServers * 3) +
            (SwitchCount * 1);

        public int FreeUnits => RackCapacityUnits - UsedUnits;

        public int TotalIops =>
            (Count12kServers * 12000) +
            (Count5kServers * 5000);

        public int TotalServerCount =>
            Count12kServers + Count5kServers;

        public double UsedUnitsPercent =>
            RackCapacityUnits == 0 ? 0 : (double)UsedUnits / RackCapacityUnits * 100.0;

        public double UsageBarWidth =>
            RackCapacityUnits == 0 ? 0 : (double)UsedUnits / RackCapacityUnits * MaxBarWidth;

        public string UsagePercentText => $"{UsedUnitsPercent:F0}%";

        public string AccentColorHex =>
            Category switch
            {
                ServerCategory.SystemX => "#F2C94C",
                ServerCategory.RISC => "#4AAEFF",
                ServerCategory.Mainframe => "#BB86FC",
                ServerCategory.GPU => "#57D68D",
                _ => "#2FA8FF"
            };
    }
}