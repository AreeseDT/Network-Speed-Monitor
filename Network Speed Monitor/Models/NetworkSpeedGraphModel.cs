using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            Results = _resultsService.GetResultsRange(Properties.Settings.Default.GraphRange);
        }

        public IEnumerable<SpeedTestResult> Results { get; private set; }

        public static DateTime XMin => DateTime.Now.Subtract(Properties.Settings.Default.GraphRange);
        public static DateTime XMax => DateTime.Now;

        public double YMax
        {
            get
            {
                if(Results.Any())
                    return Convert.ToDouble(Results.Max(x => new List<decimal> { x.Upload, x.Download, x.Ping }.Max())) * 1.1d;
                return 100;
            }
        }

        public double YInterval => YMax / 10;

        public async Task Update()
        {
            Results = await _resultsService.GetResultsRangeAsync(Properties.Settings.Default.GraphRange);
        }

        public void Reload()
        {
            Results = _resultsService.GetResultsRange(Properties.Settings.Default.GraphRange);
        }
    }
}