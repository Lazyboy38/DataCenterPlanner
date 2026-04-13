using System;
using DataCenterPlanner.Models;

namespace DataCenterPlanner.Services
{
    public class ServerCombinationOptimizer
    {
        public (int Count12k, int Count5k, int PlannedIops) FindBestCombination(
            int targetIops,
            HardwareConfig config,
            bool prefer12k)
        {
            if (!config.Allow12kServers && !config.Allow5kServers)
            {
                throw new InvalidOperationException("At least one server type must be allowed.");
            }

            int best12k = 0;
            int best5k = 0;
            int bestPlannedIops = int.MaxValue;
            int bestOvershoot = int.MaxValue;
            int bestServerCount = int.MaxValue;

            int max12k = config.Allow12kServers
                ? (int)Math.Ceiling(targetIops / (double)config.Server12kIops) + 10
                : 0;

            int max5k = config.Allow5kServers
                ? (int)Math.Ceiling(targetIops / (double)config.Server5kIops) + 20
                : 0;

            for (int count12k = 0; count12k <= max12k; count12k++)
            {
                if (count12k > 0 && !config.Allow12kServers)
                    continue;

                for (int count5k = 0; count5k <= max5k; count5k++)
                {
                    if (count5k > 0 && !config.Allow5kServers)
                        continue;

                    int plannedIops =
                        (count12k * config.Server12kIops) +
                        (count5k * config.Server5kIops);

                    if (plannedIops < targetIops)
                        continue;

                    int overshoot = plannedIops - targetIops;
                    int serverCount = count12k + count5k;

                    bool isBetter = false;

                    if (overshoot < bestOvershoot)
                    {
                        isBetter = true;
                    }
                    else if (overshoot == bestOvershoot)
                    {
                        if (serverCount < bestServerCount)
                        {
                            isBetter = true;
                        }
                        else if (serverCount == bestServerCount)
                        {
                            // Tie-breaker
                            if (prefer12k)
                            {
                                if (count12k > best12k)
                                    isBetter = true;
                            }
                            else
                            {
                                if (count5k > best5k)
                                    isBetter = true;
                            }
                        }
                    }

                    if (isBetter)
                    {
                        best12k = count12k;
                        best5k = count5k;
                        bestPlannedIops = plannedIops;
                        bestOvershoot = overshoot;
                        bestServerCount = serverCount;
                    }
                }
            }

            if (bestPlannedIops == int.MaxValue)
            {
                throw new InvalidOperationException("Could not find a valid server combination.");
            }

            return (best12k, best5k, bestPlannedIops);
        }
    }
}