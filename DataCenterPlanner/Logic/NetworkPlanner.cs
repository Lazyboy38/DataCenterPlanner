using System;
using System.Collections.Generic;
using DataCenterPlanner.Models;

namespace DataCenterPlanner.Logic
{
    public static class NetworkPlanner
    {
        public static NetworkSummary Calculate(List<RackPlan> racks, bool redundancy)
        {
            int totalRackSwitches = 0;
            int totalRj45SfpModules = 0;

            foreach (var rack in racks)
            {
                int serverCount = rack.TotalServerCount;

                int requiredPorts = redundancy
                    ? serverCount * 2
                    : serverCount;

                int rackSwitches =
                    (int)Math.Ceiling(requiredPorts / 16.0);

                totalRackSwitches += rackSwitches;
                totalRj45SfpModules += requiredPorts;
            }

            int mainSwitches = 1;

            int qsfpModules =
                (totalRackSwitches * 2) + 2;

            int qsfpPacks =
                (int)Math.Ceiling(qsfpModules / 5.0);

            int rj45SfpPacks =
                (int)Math.Ceiling(totalRj45SfpModules / 5.0);

            return new NetworkSummary
            {
                RackSwitchCount = totalRackSwitches,
                MainSwitchCount = mainSwitches,
                QsfpModuleCount = qsfpModules,
                QsfpPackCount = qsfpPacks,
                Rj45SfpModuleCount = totalRj45SfpModules,
                Rj45SfpPackCount = rj45SfpPacks
            };
        }
    }
}