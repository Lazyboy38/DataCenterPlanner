using System.Collections.Generic;
using System.Linq;

namespace DataCenterPlanner.Models
{
    public class PlannerResult
    {
        public int TotalTargetIops { get; set; }
        public int TotalPlannedIops { get; set; }

        public int Total12kServers { get; set; }
        public int Total5kServers { get; set; }
        public int TotalSwitches { get; set; }

        public List<RackPlan> Racks { get; set; } = new();

        public List<CategoryResultCard> CategoryCards =>
            Racks.GroupBy(r => r.Category)
                 .Select(group => new CategoryResultCard
                 {
                     Category = group.Key,
                     RackCount = group.Count(),
                     Total12kServers = group.Sum(r => r.Count12kServers),
                     Total5kServers = group.Sum(r => r.Count5kServers),
                     TotalSwitches = group.Sum(r => r.TotalDisplayedSwitches),
                     PlannedIops = group.Sum(r => r.TotalIops),
                     Racks = group.OrderBy(r => r.RackNumber).ToList()
                 })
                 .OrderBy(card => card.Category)
                 .ToList();

        public int TotalRacks => Racks.Count;

        public string GlobalMainRackText
        {
            get
            {
                var mainRack = Racks.FirstOrDefault(r => r.IsMainRack);
                if (mainRack == null)
                    return "No main rack assigned";

                return $"Rack {mainRack.RackNumber} ({mainRack.Category})";
            }
        }
        public NetworkSummary Network { get; set; } = new();
    }
}