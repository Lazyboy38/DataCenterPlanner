using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using DataCenterPlanner.Models;
using DataCenterPlanner.Services;
using DataCenterPlanner.Logic;
using System.Linq;

namespace DataCenterPlanner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetBuildVersion();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!TryParseNonNegative(SystemXIopsTextBox.Text, out int systemXIops) ||
                    !TryParseNonNegative(RiscIopsTextBox.Text, out int riscIops) ||
                    !TryParseNonNegative(MainframeIopsTextBox.Text, out int mainframeIops) ||
                    !TryParseNonNegative(GpuIopsTextBox.Text, out int gpuIops))
                {
                    MessageBox.Show("Please enter valid non-negative numbers for all IOPS fields.");
                    return;
                }

                var requests = new List<CategoryRequest>
                {
                    new CategoryRequest { Category = ServerCategory.SystemX, TargetIops = systemXIops },
                    new CategoryRequest { Category = ServerCategory.RISC, TargetIops = riscIops },
                    new CategoryRequest { Category = ServerCategory.Mainframe, TargetIops = mainframeIops },
                    new CategoryRequest { Category = ServerCategory.GPU, TargetIops = gpuIops }
                };

                var config = new HardwareConfig
                {
                    Allow12kServers = Allow12kServersCheckBox.IsChecked == true,
                    Allow5kServers = Allow5kServersCheckBox.IsChecked == true
                };

                if (!config.Allow12kServers && !config.Allow5kServers)
                {
                    MessageBox.Show("Please enable at least one server type.");
                    return;
                }

                string selectedMode =
                    (PlanningModeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString()
                    ?? "Marius Mode";

                IPlannerStrategy planner = selectedMode == "Minimum Racks"
                    ? new MinimumRacksPlanner()
                    : new MariusPlanner();

                var result = planner.Calculate(requests, config);
                result.Network = NetworkPlanner.Calculate(
                result.Racks,
                RedundancyCheckBox.IsChecked == true);
                result.TotalSwitches = result.Racks.Sum(r => r.TotalDisplayedSwitches);

                DataContext = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private bool TryParseNonNegative(string text, out int value)
        {
            return int.TryParse(text, out value) && value >= 0;
        }

        private void SetBuildVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var informationalVersion =
                assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            if (!string.IsNullOrWhiteSpace(informationalVersion))
            {
                BuildVersionTextBlock.Text = $"v{informationalVersion}";
                return;
            }

            var version = assembly.GetName().Version;
            if (version != null)
            {
                BuildVersionTextBlock.Text = $"v{version.Major}.{version.Minor}.{version.Build}";
            }
        }
    }
}