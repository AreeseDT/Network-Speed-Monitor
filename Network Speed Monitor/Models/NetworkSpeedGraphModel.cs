using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls.DataVisualization.Charting;
using Domain;

namespace NetworkSpeedMonitor.Models
{
    public class NetworkSpeedGraphModel
    {
        private static NetworkSpeedGraphModel _instance;
        public static NetworkSpeedGraphModel Create(SpeedTestResultsService resultsService) => 
            _instance ?? (_instance = new NetworkSpeedGraphModel(resultsService));

        private readonly SpeedTestResultsService _resultsService;

        private NetworkSpeedGraphModel(SpeedTestResultsService resultsService)
        {
            _resultsService = resultsService;
            Reload();
        }

        public List<SpeedTestResult> Results { get; private set; }

        public DateTime XMax { get; protected set; }
        public DateTime XMin { get; protected set; }

        public DateTimeIntervalType XIntervalType { get; protected set; }
        public double XInterval { get; protected set; }

        public double YMax { get; protected set; }
        public double YMin { get; } = 0.0;

        public double YInterval { get; protected set; }

        public async Task Update()
        {
            Results = await _resultsService.GetResultsRangeAsync(Properties.Settings.Default.GraphRange);
            UpdateAxes();
        }

        public void Reload()
        {
            Results = _resultsService.GetResultsRange(Properties.Settings.Default.GraphRange);
            UpdateAxes();
        }

        private void UpdateAxes()
        {
            var range = Properties.Settings.Default.GraphRange;

            if (Results.Count > 0)
            {
                XMin = Results[0].Timestamp;
                YMax = Results.Select(x => new List<decimal> {x.Upload, x.Download, x.Ping})
                           .Max(x => Convert.ToDouble(x.Max())) * 1.1;
            }
            else
            {
                XMin = DateTime.Now;
                YMax = 80;
            }

            XMax = XMin.Add(range);

            YInterval = YMax / 5;
            XIntervalType = range.Equals(TimeSpan.FromHours(1))
                ? DateTimeIntervalType.Minutes
                : DateTimeIntervalType.Hours;

            if (XIntervalType == DateTimeIntervalType.Hours)
            {
                XInterval = range.Equals(TimeSpan.FromHours(6))
                    ? 1.0
                    : 3.0;
            }
            else
            {
                XInterval = 10.0;
            }
        }
    }
}