using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace NetworkSpeedMonitor.SpeedTest
{
    public class SpeedTestWorker
    {
        private BackgroundWorker _worker;
        private readonly SpeedTestResultsService _speedTestResults;

        public SpeedTestWorker(SpeedTestResultsService speedTestResults)
        {
            _speedTestResults = speedTestResults;
            InitWorker();
        }

        public void Run()
        {
            if (_worker.CancellationPending)
            {
                InitWorker();
            }
            _worker.RunWorkerAsync();
        }

        public void Cancel()
        {
            _worker.CancelAsync();
        }

        public bool IsRunning => _worker.IsBusy && !_worker.CancellationPending;

        private async void DoWork(object sender, DoWorkEventArgs args)
        {
            var interval = Properties.Settings.Default.SpeedTestInterval;
            var lastTest = DateTime.Now.Subtract(interval);

            while (!args.Cancel)
            {
                if (DateTime.Now - lastTest > interval)
                {

                    var speedTest = new SpeedTest();
                    var results = await speedTest.Run();

                    lastTest = results.Timestamp;

                    await _speedTestResults.AddResult(results);

                    await InvokeNewResultEvent().ConfigureAwait(false);
                }
                else
                {
                    var sleepTime = interval - (DateTime.Now - lastTest);
                    Thread.Sleep(sleepTime);
                }
            }
        }

        public delegate Task OnNewResult();
        public event OnNewResult NewResult;

        private async Task InvokeNewResultEvent()
        {
            if (NewResult != null)
            {
                await NewResult.Invoke();
            }
        }

        private void InitWorker()
        {
            if (_worker != null && !_worker.CancellationPending)
            {
                _worker.CancelAsync();
            }

            _worker = new BackgroundWorker { WorkerSupportsCancellation = true };
            _worker.DoWork += DoWork;
        }
    }
}