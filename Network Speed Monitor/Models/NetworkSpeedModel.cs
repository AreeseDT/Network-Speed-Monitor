using System.ComponentModel;
using System.Threading.Tasks;
using Domain;
using NetworkSpeedMonitor.SpeedTest;

namespace NetworkSpeedMonitor.Models
{
    public class NetworkSpeedModel : INotifyPropertyChanged
    {
        private static NetworkSpeedModel _instance;
        public static NetworkSpeedModel Create(Database database) => _instance ?? (_instance = new NetworkSpeedModel(database));

        private NetworkSpeedModel(Database database)
        {
            _resultsService = SpeedTestResultsService.Create(database);

            Graph = NetworkSpeedGraphModel.Create(_resultsService);
            OnPropertyChanged(nameof(Graph));

            _speedTestWorker = new SpeedTestWorker(_resultsService);
            _speedTestWorker.NewResult += UpdateResults;
        }

        private readonly SpeedTestResultsService _resultsService;
        private readonly SpeedTestWorker _speedTestWorker;

        public NetworkSpeedGraphModel Graph { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void StartSpeedTest() => _speedTestWorker.Run();
        public void StopSpeedTest() => _speedTestWorker.Cancel();

        public bool IsSpeedTestRunning => _speedTestWorker.IsRunning;

        private async Task UpdateResults()
        {
            await Graph.Update();
            OnPropertyChanged(nameof(Graph));
        }

        public void ReloadGraph()
        {
            Graph.Reload();
            OnPropertyChanged(nameof(Graph));
        }

        public async Task ExportCsv(string file)
        {
            var results = await _resultsService.GetResultsAsync();

            CsvLogger<SpeedTestResult>.WriteFile(results, file);
        }
    }
}