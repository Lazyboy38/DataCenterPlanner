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
            LoadSettings();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
            base.OnClosing(e);
        }

        private void LoadSettings()
        {
            var s = SettingsService.Load();
            PlanningModeComboBox.SelectedIndex = s.PlanningModeIndex;
            RedundancyCheckBox.IsChecked = s.Redundancy;
            Allow12kServersCheckBox.IsChecked = s.Allow12kServers;
            Allow5kServersCheckBox.IsChecked = s.Allow5kServers;
            SystemXIopsTextBox.Text = s.SystemXIops;
            RiscIopsTextBox.Text = s.RiscIops;
            MainframeIopsTextBox.Text = s.MainframeIops;
            GpuIopsTextBox.Text = s.GpuIops;
        }

        private void SaveSettings()
        {
            SettingsService.Save(new Models.UserSettings
            {
                PlanningModeIndex = PlanningModeComboBox.SelectedIndex,
                Redundancy = RedundancyCheckBox.IsChecked == true,
                Allow12kServers = Allow12kServersCheckBox.IsChecked == true,
                Allow5kServers = Allow5kServersCheckBox.IsChecked == true,
                SystemXIops = SystemXIopsTextBox.Text,
                RiscIops = RiscIopsTextBox.Text,
                MainframeIops = MainframeIopsTextBox.Text,
                GpuIops = GpuIopsTextBox.Text
            });
        }

        private void RackViewScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainScrollViewer == null)
                return;

            MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void ShoppingScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainScrollViewer == null)
                return;

            MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void ResultScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainScrollViewer == null)
                return;

            MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
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
                result.Shopping = ShoppingPlanner.CalculateSummary(result);

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