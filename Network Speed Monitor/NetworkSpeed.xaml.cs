﻿using System.Threading;
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

        public NetworkSpeed()
        {
            InitializeComponent();
        }

        public NetworkSpeedModel Model => NetworkSpeedModel.Instance;

        private void OpenSettings_OnClick(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private void RunSpeedTest_OnChecked(object sender, RoutedEventArgs e)
        {
            Model.StartSpeedTest();
        }

        private void RunSpeedTest_OnUnchecked(object sender, RoutedEventArgs e)
        {
            Model.StopSpeedTest();
        }

        private void ExportResults(object sender, RoutedEventArgs e)
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
                Task.Run(() => Model.ExportCsv(saveFileDialog.FileName));
            }
        }
    }
}