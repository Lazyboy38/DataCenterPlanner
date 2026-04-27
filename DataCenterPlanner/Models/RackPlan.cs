using System.Collections.Generic;

namespace DataCenterPlanner.Models
{
    public class RackPlan
    {
        private const double MaxBarWidth = 220.0;
        private const double RackCardBarMaxWidth = 180.0;
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

        public string CategoryDisplayName =>
            Category switch
            {
                ServerCategory.SystemX => "System X",
                ServerCategory.RISC => "RISC",
                ServerCategory.Mainframe => "Mainframe",
                ServerCategory.GPU => "GPU",
                _ => "Unknown"
            };

        public string RackTitle => IsMainRack ? $"RACK {RackNumber} — Gateway" : $"RACK {RackNumber}";

        public string AccentColorHex =>
            Category switch
            {
                ServerCategory.SystemX => "#F2C94C",
                ServerCategory.RISC => "#4AAEFF",
                ServerCategory.Mainframe => "#BB86FC",
                ServerCategory.GPU => "#57D68D",
                _ => "#2FA8FF"
            };
        public List<RackSlotItem> Slots
        {
            get
            {
                var slots = new List<RackSlotItem>();
                int u = 1;

                slots.Add(new RackSlotItem { StartU = u, HeightU = 1, Name = "Distribution Switch A", ColorHex = "#1A5C6E" });
                u += 1;

                if (IsMainRack)
                {
                    slots.Add(new RackSlotItem { StartU = u, HeightU = 1, Name = "Main Switch", ColorHex = "#3E9EE0" });
                    u += 1;
                }
                else
                {
                    for (int i = 1; i < TotalDisplayedSwitches; i++)
                    {
                        slots.Add(new RackSlotItem { StartU = u, HeightU = 1, Name = "Distribution Switch B", ColorHex = "#1A5C6E" });
                        u += 1;
                    }
                }

                int infraEnd = u;
                int serverUnits = (Count12kServers * 7) + (Count5kServers * 3);
                int serverStartU = RackCapacityUnits - serverUnits + 1;

                if (serverStartU > infraEnd)
                {
                    int freeUnits = serverStartU - infraEnd;
                    slots.Add(new RackSlotItem { StartU = infraEnd, HeightU = freeUnits, Name = $"{freeUnits}U free", ColorHex = "#0E141C", IsFree = true });
                }

                string serverLabelColor = Category switch
                {
                    ServerCategory.SystemX => "#1C1200",
                    ServerCategory.GPU     => "#001C0A",
                    _                      => "White"
                };

                int su = serverStartU;
                for (int i = 0; i < Count12kServers; i++)
                {
                    slots.Add(new RackSlotItem { StartU = su, HeightU = 7, Name = $"{CategoryDisplayName} Large", ColorHex = AccentColorHex, LabelColorHex = serverLabelColor });
                    su += 7;
                }
                for (int i = 0; i < Count5kServers; i++)
                {
                    slots.Add(new RackSlotItem { StartU = su, HeightU = 3, Name = $"{CategoryDisplayName} Small", ColorHex = AccentColorHex, LabelColorHex = serverLabelColor });
                    su += 3;
                }

                return slots;
            }
        }

        public int VisualUsedUnits => TotalDisplayedSwitches + (Count12kServers * 7) + (Count5kServers * 3);

        public string UsageSummary =>
            $"{VisualUsedUnits}/47U · {(VisualUsedUnits * 100.0 / RackCapacityUnits):F0}%";

        public double VisualBarWidth =>
            (double)VisualUsedUnits / RackCapacityUnits * RackCardBarMaxWidth;
    }
}