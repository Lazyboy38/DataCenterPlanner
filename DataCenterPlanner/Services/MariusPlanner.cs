using System;
using System.Collections.Generic;
using System.Linq;
using DataCenterPlanner.Models;

namespace DataCenterPlanner.Services
{
    public class MariusPlanner : IPlannerStrategy
    {
        public PlannerResult Calculate(List<CategoryRequest> requests, HardwareConfig config)
        {
            var result = new PlannerResult();
            int globalRackNumber = 1;

            foreach (var request in requests.Where(r => r.TargetIops > 0))
            {
                int remainingIops = request.TargetIops;
                var categoryRacks = new List<RackPlan>();

                while (remainingIops >= 60000)
                {
                    categoryRacks.Add(new RackPlan
                    {
                        RackNumber = globalRackNumber++,
                        Category = request.Category,
                        Count12kServers = 5,
                        Count5kServers = 0,
                        SwitchCount = 1
                    });

                    remainingIops -= 60000;
                }

                if (remainingIops > 0)
                {
                    var remainderRack = new RackPlan
                    {
                        RackNumber = globalRackNumber++,
                        Category = request.Category,
                        Count12kServers = 0,
                        Count5kServers = 0,
                        SwitchCount = 1
                    };

                    while (remainingIops > 0)
                    {
                        if (remainingIops >= config.Server12kIops)
                        {
                            int hypotheticalUnits =
                                ((remainderRack.Count12kServers + 1) * config.Server12kUnits) +
                                (remainderRack.Count5kServers * config.Server5kUnits) +
                                (remainderRack.SwitchCount * config.SwitchUnits);

                            if (hypotheticalUnits <= config.RackUnits)
                            {
                                remainderRack.Count12kServers++;
                                remainingIops -= config.Server12kIops;
                                continue;
                            }
                        }

                        int hypothetical5kUnits =
                            (remainderRack.Count12kServers * config.Server12kUnits) +
                            ((remainderRack.Count5kServers + 1) * config.Server5kUnits) +
                            (remainderRack.SwitchCount * config.SwitchUnits);

                        if (hypothetical5kUnits <= config.RackUnits)
                        {
                            remainderRack.Count5kServers++;
                            remainingIops -= config.Server5kIops;
                        }
                        else
                        {
                            categoryRacks.Add(remainderRack);

                            remainderRack = new RackPlan
                            {
                                RackNumber = globalRackNumber++,
                                Category = request.Category,
                                Count12kServers = 0,
                                Count5kServers = 0,
                                SwitchCount = 1
                            };
                        }
                    }

                    categoryRacks.Add(remainderRack);
                }

                result.Racks.AddRange(categoryRacks);
            }

            if (result.Racks.Count > 0)
            {
                var mainRack = result.Racks
                    .OrderBy(r => r.TotalServerCount)
                    .ThenBy(r => r.UsedUnits)
                    .First();

                mainRack.IsMainRack = true;
                mainRack.SwitchCount += 1;

                if (mainRack.UsedUnits > config.RackUnits)
                {
                    throw new InvalidOperationException(
                        $"Global main rack {mainRack.RackNumber} exceeds rack capacity after adding second switch.");
                }
            }

            result.TotalTargetIops = requests.Sum(r => r.TargetIops);
            result.TotalPlannedIops = result.Racks.Sum(r => r.TotalIops);
            result.Total12kServers = result.Racks.Sum(r => r.Count12kServers);
            result.Total5kServers = result.Racks.Sum(r => r.Count5kServers);
            result.TotalSwitches = result.Racks.Sum(r => r.SwitchCount);

            return result;
        }
    }
}