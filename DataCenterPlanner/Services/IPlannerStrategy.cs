using System.Collections.Generic;
using DataCenterPlanner.Models;

namespace DataCenterPlanner.Services
{
    public interface IPlannerStrategy
    {
        PlannerResult Calculate(List<CategoryRequest> requests, HardwareConfig config);
    }
}