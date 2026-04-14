using System.Collections.Generic;
using DataCenterPlanner.Models;

namespace DataCenterPlanner.Services
{
    public static class ProductCatalog
    {
        public static readonly Dictionary<string, ProductCatalogItem> Items = new()
        {
            // Compute - SystemX
            ["systemx_5k"] = new ProductCatalogItem
            {
                Id = "systemx_5k",
                DisplayName = "SystemX 5K Server",
                Category = ProductCategory.Compute,
                UnitPrice = 400,
                PackSize = 1,
                RackUnits = 3,
                Iops = 5000,
                IsCalculated = true
            },
            ["systemx_12k"] = new ProductCatalogItem
            {
                Id = "systemx_12k",
                DisplayName = "SystemX 12K Server",
                Category = ProductCategory.Compute,
                UnitPrice = 1600,
                PackSize = 1,
                RackUnits = 7,
                Iops = 12000,
                IsCalculated = true
            },

            // Compute - RISC
            ["risc_5k"] = new ProductCatalogItem
            {
                Id = "risc_5k",
                DisplayName = "RISC 5K Server",
                Category = ProductCategory.Compute,
                UnitPrice = 450,
                PackSize = 1,
                RackUnits = 3,
                Iops = 5000,
                IsCalculated = true
            },
            ["risc_12k"] = new ProductCatalogItem
            {
                Id = "risc_12k",
                DisplayName = "RISC 12K Server",
                Category = ProductCategory.Compute,
                UnitPrice = 1750,
                PackSize = 1,
                RackUnits = 7,
                Iops = 12000,
                IsCalculated = true
            },

            // Compute - Mainframe
            ["mainframe_5k"] = new ProductCatalogItem
            {
                Id = "mainframe_5k",
                DisplayName = "Mainframe 5K Server",
                Category = ProductCategory.Compute,
                UnitPrice = 850,
                PackSize = 1,
                RackUnits = 3,
                Iops = 5000,
                IsCalculated = true
            },
            ["mainframe_12k"] = new ProductCatalogItem
            {
                Id = "mainframe_12k",
                DisplayName = "Mainframe 12K Server",
                Category = ProductCategory.Compute,
                UnitPrice = 2000,
                PackSize = 1,
                RackUnits = 7,
                Iops = 12000,
                IsCalculated = true
            },

            // Compute - GPU
            ["gpu_5k"] = new ProductCatalogItem
            {
                Id = "gpu_5k",
                DisplayName = "GPU 5K Server",
                Category = ProductCategory.Compute,
                UnitPrice = 550,
                PackSize = 1,
                RackUnits = 3,
                Iops = 5000,
                IsCalculated = true
            },
            ["gpu_12k"] = new ProductCatalogItem
            {
                Id = "gpu_12k",
                DisplayName = "GPU 12K Server",
                Category = ProductCategory.Compute,
                UnitPrice = 2200,
                PackSize = 1,
                RackUnits = 7,
                Iops = 12000,
                IsCalculated = true
            },

            // Switches
            ["switch_rj45_16"] = new ProductCatalogItem
            {
                Id = "switch_rj45_16",
                DisplayName = "16x 10Gbps RJ45 Switch",
                Category = ProductCategory.Switch,
                UnitPrice = 250,
                PackSize = 1,
                RackUnits = 1,
                SpeedGbps = 10,
                IsCalculated = false
            },
            ["switch_sfp_4"] = new ProductCatalogItem
            {
                Id = "switch_sfp_4",
                DisplayName = "4x SFP+/SFP28 Switch",
                Category = ProductCategory.Switch,
                UnitPrice = 400,
                PackSize = 1,
                RackUnits = 1,
                SpeedGbps = 25,
                IsCalculated = false
            },
            ["switch_qsfp_32"] = new ProductCatalogItem
            {
                Id = "switch_qsfp_32",
                DisplayName = "32x QSFP+ Switch",
                Category = ProductCategory.Switch,
                UnitPrice = 3800,
                PackSize = 1,
                RackUnits = 1,
                SpeedGbps = 40,
                IsCalculated = true
            },
            ["switch_combo_qsfp_sfp"] = new ProductCatalogItem
            {
                Id = "switch_combo_qsfp_sfp",
                DisplayName = "4x QSFP+ + 16x SFP+/SFP28 Switch",
                Category = ProductCategory.Switch,
                UnitPrice = 3500,
                PackSize = 1,
                RackUnits = 1,
                SpeedGbps = 40,
                IsCalculated = true
            },

            // Rack
            ["rack_47u"] = new ProductCatalogItem
            {
                Id = "rack_47u",
                DisplayName = "Lanberg Rack Cabinet 19\" 47U/800x800",
                Category = ProductCategory.Rack,
                UnitPrice = 1250,
                PackSize = 1,
                RackUnits = 47,
                IsCalculated = true
            },

            // Patch Panels
            ["patch_rj45"] = new ProductCatalogItem
            {
                Id = "patch_rj45",
                DisplayName = "Patch Panel RJ45",
                Category = ProductCategory.PatchPanel,
                UnitPrice = 250,
                PackSize = 1,
                IsCalculated = false
            },
            ["patch_combo"] = new ProductCatalogItem
            {
                Id = "patch_combo",
                DisplayName = "Patch Panel Combo",
                Category = ProductCategory.PatchPanel,
                UnitPrice = 450,
                PackSize = 1,
                IsCalculated = false
            },
            ["patch_fiber"] = new ProductCatalogItem
            {
                Id = "patch_fiber",
                DisplayName = "Patch Panel Fiber",
                Category = ProductCategory.PatchPanel,
                UnitPrice = 450,
                PackSize = 1,
                IsCalculated = false
            },

            // Cable
            ["cable_cat6e"] = new ProductCatalogItem
            {
                Id = "cable_cat6e",
                DisplayName = "Cable Copper CAT6E",
                Category = ProductCategory.Cable,
                UnitPrice = 500,
                PackSize = 1,
                SpeedGbps = 10,
                IsCalculated = false
            },
            ["cable_fiber_1lane"] = new ProductCatalogItem
            {
                Id = "cable_fiber_1lane",
                DisplayName = "Cable Fiber 1 Lane",
                Category = ProductCategory.Cable,
                UnitPrice = 1000,
                PackSize = 1,
                SpeedGbps = 10,
                IsCalculated = false
            },
            ["cable_fiber_4lane"] = new ProductCatalogItem
            {
                Id = "cable_fiber_4lane",
                DisplayName = "Cable Fiber 4 Lanes",
                Category = ProductCategory.Cable,
                UnitPrice = 3000,
                PackSize = 1,
                SpeedGbps = 40,
                IsCalculated = false
            },

            // Modules
            ["sfp_rj45_pack"] = new ProductCatalogItem
            {
                Id = "sfp_rj45_pack",
                DisplayName = "5x SFP+ Module RJ45",
                Category = ProductCategory.Module,
                UnitPrice = 250,
                PackSize = 5,
                SpeedGbps = 10,
                IsCalculated = true
            },
            ["sfp_fiber_10_pack"] = new ProductCatalogItem
            {
                Id = "sfp_fiber_10_pack",
                DisplayName = "5x SFP+ Module Fiber 10Gbps",
                Category = ProductCategory.Module,
                UnitPrice = 350,
                PackSize = 5,
                SpeedGbps = 10,
                IsCalculated = false
            },
            ["sfp28_fiber_25_pack"] = new ProductCatalogItem
            {
                Id = "sfp28_fiber_25_pack",
                DisplayName = "5x SFP28 Module Fiber 25Gbps",
                Category = ProductCategory.Module,
                UnitPrice = 900,
                PackSize = 5,
                SpeedGbps = 25,
                IsCalculated = false
            },
            ["qsfp_fiber_40_pack"] = new ProductCatalogItem
            {
                Id = "qsfp_fiber_40_pack",
                DisplayName = "5x QSFP+ Module Fiber 40Gbps",
                Category = ProductCategory.Module,
                UnitPrice = 1500,
                PackSize = 5,
                SpeedGbps = 40,
                IsCalculated = true
            }
        };
    }
}