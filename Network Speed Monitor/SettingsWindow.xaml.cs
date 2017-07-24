using System.Windows;
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
            Model = new SettingsModel();
            InitializeComponent();
        }

        public SettingsModel Model { get; private set; }

        private void SpeedTestInterval_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Model.IsNumericInput(e.Text);
        }
    }
}
