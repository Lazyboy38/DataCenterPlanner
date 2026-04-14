using System.Linq;
using DataCenterPlanner.Models;

namespace DataCenterPlanner.Services
{
    public static class ShoppingPlanner
    {
        public static ShoppingSummary CalculateSummary(PlannerResult result)
        {
            decimal computeCost = 0;
            decimal infrastructureCost = 0;
            decimal moduleCost = 0;

            foreach (var rack in result.Racks)
            {
                string categoryPrefix = rack.Category switch
                {
                    ServerCategory.SystemX => "systemx",
                    ServerCategory.RISC => "risc",
                    ServerCategory.Mainframe => "mainframe",
                    ServerCategory.GPU => "gpu",
                    _ => ""
                };

                if (rack.Count5kServers > 0)
                {
                    computeCost += rack.Count5kServers *
                                   ProductCatalog.Items[$"{categoryPrefix}_5k"].UnitPrice;
                }

                if (rack.Count12kServers > 0)
                {
                    computeCost += rack.Count12kServers *
                                   ProductCatalog.Items[$"{categoryPrefix}_12k"].UnitPrice;
                }
            }

            infrastructureCost += result.TotalRacks *
                                  ProductCatalog.Items["rack_47u"].UnitPrice;

            infrastructureCost += result.Network.RackSwitchCount *
                                  ProductCatalog.Items["switch_combo_qsfp_sfp"].UnitPrice;

            infrastructureCost += result.Network.MainSwitchCount *
                                  ProductCatalog.Items["switch_qsfp_32"].UnitPrice;

            moduleCost += result.Network.Rj45SfpPackCount *
                          ProductCatalog.Items["sfp_rj45_pack"].UnitPrice;

            moduleCost += result.Network.QsfpPackCount *
                          ProductCatalog.Items["qsfp_fiber_40_pack"].UnitPrice;

            return new ShoppingSummary
            {
                ComputeCost = computeCost,
                InfrastructureCost = infrastructureCost,
                ModuleCost = moduleCost
            };
        }
    }
}