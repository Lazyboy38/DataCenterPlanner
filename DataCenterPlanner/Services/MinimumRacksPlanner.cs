using System;
using System.Collections.Generic;
using System.Linq;
using DataCenterPlanner.Models;

namespace DataCenterPlanner.Services
{
    public class MinimumRacksPlanner : IPlannerStrategy
    {
        public PlannerResult Calculate(List<CategoryRequest> requests, HardwareConfig config)
        {
            var result = new PlannerResult();
            int globalRackNumber = 1;

            foreach (var request in requests.Where(r => r.TargetIops > 0))
            {
                int remainingIops = request.TargetIops;
                var categoryRacks = new List<RackPlan>();

                while (remainingIops > 0)
                {
                    var rack = new RackPlan
                    {
                        RackNumber = globalRackNumber++,
                        Category = request.Category,
                        Count12kServers = 0,
                        Count5kServers = 0,
                        SwitchCount = 1
                    };

                    bool addedSomething;

                    do
                    {
                        addedSomething = false;

                        int hypothetical12kUnits =
                            ((rack.Count12kServers + 1) * config.Server12kUnits) +
                            (rack.Count5kServers * config.Server5kUnits) +
                            (rack.SwitchCount * config.SwitchUnits);

                        if (hypothetical12kUnits <= config.RackUnits)
                        {
                            rack.Count12kServers++;
                            remainingIops -= config.Server12kIops;
                            addedSomething = true;

                            if (remainingIops <= 0)
                                break;
                        }

                        int hypothetical5kUnits =
                            (rack.Count12kServers * config.Server12kUnits) +
                            ((rack.Count5kServers + 1) * config.Server5kUnits) +
                            (rack.SwitchCount * config.SwitchUnits);

                        if (remainingIops > 0 && hypothetical5kUnits <= config.RackUnits)
                        {
                            rack.Count5kServers++;
                            remainingIops -= config.Server5kIops;
                            addedSomething = true;
                        }

                    } while (addedSomething && remainingIops > 0);

                    if (rack.TotalServerCount == 0)
                    {
                        throw new InvalidOperationException(
                            $"Could not place any server for {request.Category}.");
                    }

                    categoryRacks.Add(rack);
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