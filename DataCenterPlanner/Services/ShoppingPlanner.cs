using System.Collections.Generic;
using System.Linq;
using DataCenterPlanner.Models;

namespace DataCenterPlanner.Services
{
    public static class ShoppingPlanner
    {
        public static ShoppingSummary CalculateSummary(PlannerResult result)
        {
            var summary = new ShoppingSummary();

            var computeItems = BuildComputeItems(result);
            var infrastructureItems = BuildInfrastructureItems(result);
            var moduleItems = BuildModuleItems(result);

            summary.ComputeCost = computeItems.Sum(i => i.TotalPrice);
            summary.InfrastructureCost = infrastructureItems.Sum(i => i.TotalPrice);
            summary.ModuleCost = moduleItems.Sum(i => i.TotalPrice);

            summary.Categories.Add(new ShoppingCategory
            {
                Name = "Compute",
                AccentColorHex = "#F2C94C",
                Items = computeItems
            });

            summary.Categories.Add(new ShoppingCategory
            {
                Name = "Infrastructure",
                AccentColorHex = "#4AAEFF",
                Items = infrastructureItems
            });

            summary.Categories.Add(new ShoppingCategory
            {
                Name = "Modules",
                AccentColorHex = "#57D68D",
                Items = moduleItems
            });

            return summary;
        }

        private static List<ShoppingItem> BuildComputeItems(PlannerResult result)
        {
            var quantities = new Dictionary<string, int>();

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
                    string key = $"{categoryPrefix}_5k";
                    quantities[key] = quantities.GetValueOrDefault(key) + rack.Count5kServers;
                }

                if (rack.Count12kServers > 0)
                {
                    string key = $"{categoryPrefix}_12k";
                    quantities[key] = quantities.GetValueOrDefault(key) + rack.Count12kServers;
                }
            }

            return quantities
                .OrderBy(kvp => ProductCatalog.Items[kvp.Key].DisplayName)
                .Select(kvp => new ShoppingItem
                {
                    Name = ProductCatalog.Items[kvp.Key].DisplayName,
                    Category = "Compute",
                    Quantity = kvp.Value,
                    UnitPrice = ProductCatalog.Items[kvp.Key].UnitPrice
                })
                .ToList();
        }

        private static List<ShoppingItem> BuildInfrastructureItems(PlannerResult result)
        {
            var items = new List<ShoppingItem>();

            if (result.TotalRacks > 0)
            {
                items.Add(new ShoppingItem
                {
                    Name = ProductCatalog.Items["rack_47u"].DisplayName,
                    Category = "Infrastructure",
                    Quantity = result.TotalRacks,
                    UnitPrice = ProductCatalog.Items["rack_47u"].UnitPrice
                });
            }

            if (result.Network.RackSwitchCount > 0)
            {
                items.Add(new ShoppingItem
                {
                    Name = ProductCatalog.Items["switch_combo_qsfp_sfp"].DisplayName,
                    Category = "Infrastructure",
                    Quantity = result.Network.RackSwitchCount,
                    UnitPrice = ProductCatalog.Items["switch_combo_qsfp_sfp"].UnitPrice
                });
            }

            if (result.Network.MainSwitchCount > 0)
            {
                items.Add(new ShoppingItem
                {
                    Name = ProductCatalog.Items["switch_qsfp_32"].DisplayName,
                    Category = "Infrastructure",
                    Quantity = result.Network.MainSwitchCount,
                    UnitPrice = ProductCatalog.Items["switch_qsfp_32"].UnitPrice
                });
            }

            return items;
        }

        private static List<ShoppingItem> BuildModuleItems(PlannerResult result)
        {
            var items = new List<ShoppingItem>();

            if (result.Network.Rj45SfpPackCount > 0)
            {
                items.Add(new ShoppingItem
                {
                    Name = ProductCatalog.Items["sfp_rj45_pack"].DisplayName,
                    Category = "Modules",
                    Quantity = result.Network.Rj45SfpPackCount,
                    UnitPrice = ProductCatalog.Items["sfp_rj45_pack"].UnitPrice
                });
            }

            if (result.Network.QsfpPackCount > 0)
            {
                items.Add(new ShoppingItem
                {
                    Name = ProductCatalog.Items["qsfp_fiber_40_pack"].DisplayName,
                    Category = "Modules",
                    Quantity = result.Network.QsfpPackCount,
                    UnitPrice = ProductCatalog.Items["qsfp_fiber_40_pack"].UnitPrice
                });
            }

            return items;
        }
    }
}