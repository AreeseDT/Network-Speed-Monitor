using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace NetworkSpeedMonitor.Models
{
    public class NetworkSpeedModel : INotifyPropertyChanged
    {
        private static NetworkSpeedModel _instance;
        public static NetworkSpeedModel Create(Database database)
        {
            return _instance ?? (_instance = new NetworkSpeedModel(database));
        }

        private readonly Database _database;
        private static readonly Mutex Lock = new Mutex(true, "dbLock");

        private NetworkSpeedModel(Database database)
        {
            _database = database;

            SpeedTestResults = GetSpeedTestResults();

            OnPropertyChanged(nameof(SpeedTestResults));

            Lock.ReleaseMutex();
        }

        public IEnumerable<SpeedTestResult> SpeedTestResults { get; set; }

        protected CancellationTokenSource Cancellation { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private IEnumerable<SpeedTestResult> GetSpeedTestResults()
        {
            var cutoff = DateTime.Now.Subtract(TimeSpan.FromHours(12));

            return _database.MostRecentResults(cutoff, 100);
        }

        public void StartSpeedTest()
        {
            Cancellation = new CancellationTokenSource();
            Cancellation.Token.ThrowIfCancellationRequested();
            Task.Run(() =>
            {
                var interval = Properties.Settings.Default.SpeedTestInterval;
                var lastTest = DateTime.Now.Subtract(interval);
                
                while (!Cancellation.IsCancellationRequested)
                {
                    if (DateTime.Now - lastTest > interval)
                    {
                        lastTest = DateTime.Now;
                        var speedTest = new SpeedTest();
                        var results = speedTest.Run(Cancellation.Token);
                        results.Timestamp = lastTest;

                        try
                        {
                            Lock.WaitOne();

                            _database.SpeedTestResults.Add(results);
                            _database.SaveChanges();

                            SpeedTestResults = GetSpeedTestResults();

                            OnPropertyChanged(nameof(SpeedTestResults));
                        }
                        finally
                        {
                            Lock.ReleaseMutex();
                        }
                    }
                    else
                    {
                        var sleepTime = interval - (DateTime.Now - lastTest);
                        Thread.Sleep(sleepTime);
                    }
                }
            }, Cancellation.Token);
        }

        public void StopSpeedTest()
        {
            Cancellation?.Cancel();
        }

        public async Task ExportCsv(string file)
        {
            Lock.WaitOne();

            var results = await _database.SpeedTestResults.ToListAsync();

            Lock.ReleaseMutex();

            CsvLogger<SpeedTestResult>.WriteFile(results, file);
        }
    }
}