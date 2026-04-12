namespace DataCenterPlanner.Models
{
    public class NetworkSummary
    {
        public int RackSwitchCount { get; set; }

        public int MainSwitchCount { get; set; }

        public int QsfpModuleCount { get; set; }

        public int QsfpPackCount { get; set; }

        public int Rj45SfpModuleCount { get; set; }

        public int Rj45SfpPackCount { get; set; }

        public int TotalNetworkCost =>
            (RackSwitchCount * 3500) +
            (MainSwitchCount * 3800) +
            (QsfpPackCount * 1500) +
            (Rj45SfpPackCount * 250);
    }
}