using System;
using System.Text.RegularExpressions;

namespace NetworkSpeedMonitor.Models
{
    public class SettingsModel
    {
        private static readonly Regex IsNumeric;

        static SettingsModel()
        {
            IsNumeric = new Regex(@"[^0-9.\-,]+", RegexOptions.Compiled);
        }

        public string SpeedTestInterval
        {
            get => Properties.Settings.Default.SpeedTestInterval.TotalMinutes.ToString();
            set
            {
                double.TryParse(value, out double minutes);
                var interval = TimeSpan.FromMinutes(minutes);
                Properties.Settings.Default.SpeedTestInterval = interval;
                Properties.Settings.Default.Save();
            }
        }

        public bool IsNumericInput(string text) => IsNumeric.IsMatch(text);
    }
}