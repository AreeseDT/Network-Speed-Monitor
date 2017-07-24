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
    public class NetworkSpeedModel
    {
        private static NetworkSpeedModel _instance;
        public static NetworkSpeedModel Instance => _instance ?? (_instance = new NetworkSpeedModel());

        private readonly Database _database;
        private static readonly Mutex Lock = new Mutex(true, "dbLock");

        private NetworkSpeedModel()
        {
            _database = new Database();
            Lock.ReleaseMutex();
        }

        protected CancellationTokenSource Cancellation { get; set; }

        public async Task<IEnumerable<SpeedTestResult>> GetSpeedTestResults()
        {
            Lock.WaitOne();
            var query = _database.SpeedTestResults.AsQueryable();
            var count = await query.CountAsync();

            var results = count <= 100
                ? await query.ToListAsync()
                : await query.Skip(count - 100).ToListAsync();

            Lock.ReleaseMutex();

            return results;
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

                            _database.SpeedTestResults.AddAsync(results);
                            _database.SaveChanges();
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