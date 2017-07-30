using System;
using System.Threading.Tasks;
using System.Windows;
using Domain;
using Microsoft.Win32;
using NetworkSpeedMonitor.Models;

namespace NetworkSpeedMonitor
{
    /// <summary>
    /// Interaction logic for NetworkSpeed.xaml
    /// </summary>
    public partial class NetworkSpeed : Window
    {
        private readonly Settings _settings;
        public NetworkSpeed(Database database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            Model = NetworkSpeedModel.Create(database);
            _settings = new Settings();

            InitializeComponent();
        }

        public NetworkSpeedModel Model { get; private set; }

        private void OpenSettings_OnClick(object sender, RoutedEventArgs e)
        {
            var testInterval = _settings.SpeedTestInterval;
            var graphRange = _settings.GraphRange;

            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();

            if (!testInterval.Equals(_settings.SpeedTestInterval) && Model.IsSpeedTestRunning)
            {
                Model.StopSpeedTest();
                Model.StartSpeedTest();
            }

            if (!graphRange.Equals(_settings.GraphRange))
            {
                Model.ReloadGraph();
            }
        }

        private void RunSpeedTest_OnChecked(object sender, RoutedEventArgs e)
        {
            Model.StartSpeedTest();
        }

        private void RunSpeedTest_OnUnchecked(object sender, RoutedEventArgs e)
        {
            Model.StopSpeedTest();
        }

        private async void ExportResults(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".csv",
                AddExtension = true,
                FileName = "SpeedResults.csv",
                OverwritePrompt = true,
                RestoreDirectory = true,
                Filter = "Csv File (*.csv)|*.csv"
            };

            if (saveFileDialog.ShowDialog().GetValueOrDefault())
            {
                await Model.ExportCsv(saveFileDialog.FileName);
            }
        }
    }
}
