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
            if (!config.Allow12kServers && !config.Allow5kServers)
            {
                throw new InvalidOperationException("At least one server type must be allowed.");
            }

            var result = new PlannerResult();
            int globalRackNumber = 1;

            foreach (var request in requests.Where(r => r.TargetIops > 0))
            {
                int remainingIops = request.TargetIops;
                var categoryRacks = new List<RackPlan>();

                // Fall 1: Nur 12k erlaubt
                if (config.Allow12kServers && !config.Allow5kServers)
                {
                    int required12k = (int)Math.Ceiling(remainingIops / (double)config.Server12kIops);

                    while (required12k > 0)
                    {
                        var rack = new RackPlan
                        {
                            RackNumber = globalRackNumber++,
                            Category = request.Category,
                            Count12kServers = Math.Min(5, required12k),
                            Count5kServers = 0,
                            SwitchCount = 1
                        };

                        required12k -= rack.Count12kServers;
                        categoryRacks.Add(rack);
                    }
                }
                // Fall 2: Nur 5k erlaubt
                else if (!config.Allow12kServers && config.Allow5kServers)
                {
                    int required5k = (int)Math.Ceiling(remainingIops / (double)config.Server5kIops);

                    while (required5k > 0)
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

                        while (required5k > 0 &&
                               usedUnits + config.Server5kUnits <= config.RackUnits)
                        {
                            rack.Count5kServers++;
                            required5k--;
                            usedUnits += config.Server5kUnits;
                        }

                        categoryRacks.Add(rack);
                    }
                }
                // Fall 3: Mixed mode -> Marius-Stil
                else
                {
                    // Erst volle 60k-Racks mit 5x12k
                    while (remainingIops > 60000)
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

                    // Rest-Rack bewusst "schön" lösen:
                    // 1. so viele 12k wie sinnvoll
                    // 2. dann mit 5k exakt oder knapp darüber schließen
                    if (remainingIops > 0)
                    {
                        RackPlan? bestRack = null;
                        int bestOvershoot = int.MaxValue;
                        int best5kCount = -1;
                        int best12kCount = -1;

                        for (int count12k = 0; count12k <= 5; count12k++)
                        {
                            int unitsFrom12k = count12k * config.Server12kUnits + config.SwitchUnits;
                            if (unitsFrom12k > config.RackUnits)
                                continue;

                            for (int count5k = 0; count5k <= 20; count5k++)
                            {
                                int usedUnits = unitsFrom12k + (count5k * config.Server5kUnits);
                                if (usedUnits > config.RackUnits)
                                    continue;

                                int plannedIops =
                                    (count12k * config.Server12kIops) +
                                    (count5k * config.Server5kIops);

                                if (plannedIops < remainingIops)
                                    continue;

                                int overshoot = plannedIops - remainingIops;

                                bool isBetter = false;

                                if (overshoot < bestOvershoot)
                                {
                                    isBetter = true;
                                }
                                else if (overshoot == bestOvershoot)
                                {
                                    // Im Marius-Mode bei Gleichstand lieber MEHR 5k im Rest-Rack,
                                    // damit die Restlösung eher "kleinteilig" und sauber wirkt
                                    if (count5k > best5kCount)
                                    {
                                        isBetter = true;
                                    }
                                    else if (count5k == best5kCount && count12k > best12kCount)
                                    {
                                        isBetter = true;
                                    }
                                }

                                if (isBetter)
                                {
                                    bestOvershoot = overshoot;
                                    best5kCount = count5k;
                                    best12kCount = count12k;

                                    bestRack = new RackPlan
                                    {
                                        RackNumber = globalRackNumber,
                                        Category = request.Category,
                                        Count12kServers = count12k,
                                        Count5kServers = count5k,
                                        SwitchCount = 1
                                    };
                                }
                            }
                        }

                        if (bestRack == null)
                        {
                            throw new InvalidOperationException(
                                $"Could not create a valid remainder rack for {request.Category}.");
                        }

                        bestRack.RackNumber = globalRackNumber++;
                        categoryRacks.Add(bestRack);
                    }
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