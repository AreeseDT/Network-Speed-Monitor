using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NetworkSpeedMonitor.Models;

namespace NetworkSpeedMonitor
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public SettingsModel Model => SettingsModel.Instance;
        
        private void SpeedTestIntervalLoaded(object sender, RoutedEventArgs e)
        {
            SpeedTestInterval.Text = Model.Settings.SpeedTestInterval.TotalMinutes.ToString();
        }

        private void SpeedTestIntervalChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(SpeedTestInterval.Text, out double min);
            Model.Settings.SpeedTestInterval = TimeSpan.FromMinutes(min);
        }

        private void SpeedTestInterval_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Model.IsNumericInput(e.Text);
        }

        private void UpdateGraphRange(object sender, SelectionChangedEventArgs e)
        {
            var selectList = sender as ComboBox;
            if (selectList == null) return;

            Model.Settings.GraphRange = (TimeSpan) selectList.SelectedItem;
        }

        private void GraphRangeOptionsLoaded(object sender, RoutedEventArgs e)
        {
            var cbox = sender as ComboBox;
            if (cbox == null) return;

            var options = new List<TimeSpan>
            {
                TimeSpan.FromHours(24),
                TimeSpan.FromHours(12),
                TimeSpan.FromHours(6),
                TimeSpan.FromHours(1)
            };
            
            cbox.ItemsSource = options;
            cbox.SelectedIndex = options.FindIndex(x => x.Equals(Model.Settings.GraphRange));
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            Model.Save();
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
