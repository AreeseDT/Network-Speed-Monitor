using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace NetworkSpeedMonitor.Models
{
    public class SettingsModel : INotifyPropertyChanged
    {
        private static SettingsModel _instance;
        public static SettingsModel Instance
        {
            get => _instance ?? (_instance = new SettingsModel());
        }

        private static readonly Regex IsNumeric;

        static SettingsModel()
        {
            IsNumeric = new Regex(@"[^0-9.\-,]+", RegexOptions.Compiled);
        }

        private SettingsModel()
        {
            Settings = new Settings();
        }

        public Settings Settings { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public void Save()
        {
            Settings.Save();
            OnPropertyChanged(nameof(Settings));
        }

        public bool IsNumericInput(string text) => IsNumeric.IsMatch(text);
    }
}