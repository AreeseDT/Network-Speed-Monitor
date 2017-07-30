using System;

namespace NetworkSpeedMonitor
{
    public class Settings
    {
        private static readonly Properties.Settings AppSettings = Properties.Settings.Default;

        public TimeSpan SpeedTestInterval
        {
            get => AppSettings.SpeedTestInterval;
            set => AppSettings.SpeedTestInterval = value;
        }

        public TimeSpan GraphRange
        {
            get => AppSettings.GraphRange;
            set => AppSettings.GraphRange = value;
        }

        public void Save()
        {
            AppSettings.Save();
        }
    }
}