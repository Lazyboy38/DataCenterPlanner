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
            if (!config.Allow12kServers && !config.Allow5kServers)
            {
                throw new InvalidOperationException("At least one server type must be allowed.");
            }

            var result = new PlannerResult();
            int globalRackNumber = 1;
            var optimizer = new ServerCombinationOptimizer();

            foreach (var request in requests.Where(r => r.TargetIops > 0))
            {
                var bestCombination = optimizer.FindBestCombination(
                    request.TargetIops,
                    config,
                    prefer12k: true);

                int remaining12k = bestCombination.Count12k;
                int remaining5k = bestCombination.Count5k;

                var categoryRacks = new List<RackPlan>();

                while (remaining12k > 0 || remaining5k > 0)
                {
                    var rack = new RackPlan
                    {
                        RackNumber = globalRackNumber++,
                        Category = request.Category,
                        Count12kServers = 0,
                        Count5kServers = 0,
                        SwitchCount = 1
                    };

                    int usedUnits = config.SwitchUnits;

                    bool placedSomething;

                    do
                    {
                        placedSomething = false;

                        if (remaining12k > 0 &&
                            usedUnits + config.Server12kUnits <= config.RackUnits)
                        {
                            rack.Count12kServers++;
                            remaining12k--;
                            usedUnits += config.Server12kUnits;
                            placedSomething = true;
                        }

                        if (remaining5k > 0 &&
                            usedUnits + config.Server5kUnits <= config.RackUnits)
                        {
                            rack.Count5kServers++;
                            remaining5k--;
                            usedUnits += config.Server5kUnits;
                            placedSomething = true;
                        }

                    } while (placedSomething);

                    if (rack.TotalServerCount == 0)
                    {
                        throw new InvalidOperationException(
                            $"Could not place any server in rack {rack.RackNumber}.");
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
            result.TotalSwitches = result.Racks.Sum(r => r.TotalDisplayedSwitches);

            return result;
        }
    }
}