using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    public class SpeedTestResultsService
    {
        private readonly Database _database;
        private static readonly Mutex Lock = new Mutex(true, "dbLock");

        private static SpeedTestResultsService _instance;
        public static SpeedTestResultsService Create(Database database)
        {
            return _instance ?? (_instance = new SpeedTestResultsService(database));
        }

        private SpeedTestResultsService(Database database)
        {
            _database = database;
            Lock.ReleaseMutex();
        }

        public async Task AddResult(SpeedTestResult result)
        {
            Lock.WaitOne();

            await _database.SpeedTestResults.AddAsync(result);
            await _database.SaveChangesAsync();

            Lock.ReleaseMutex();
        }

        public IEnumerable<SpeedTestResult> GetResults()
        {
            Lock.WaitOne();

            var results = _database.SpeedTestResults.ToList();

            Lock.ReleaseMutex();

            return results;
        }

        public async Task<IEnumerable<SpeedTestResult>> GetResultsAsync()
        {
            Lock.WaitOne();

            var results = await _database.SpeedTestResults.ToListAsync();

            Lock.ReleaseMutex();

            return results;
        }

        public IEnumerable<SpeedTestResult> GetResultsRange(TimeSpan range)
        {
            var start = DateTime.Now.Subtract(range);

            Lock.WaitOne();

            var results = _database.SpeedTestResults
                .AsQueryable()
                .OrderByDescending(x => x.Timestamp)
                .Where(x => x.Timestamp >= start)
                .ToList();

            Lock.ReleaseMutex();

            return results;
        }

        public async Task<IEnumerable<SpeedTestResult>> GetResultsRangeAsync(TimeSpan range)
        {
            var start = DateTime.Now.Subtract(range);

            Lock.WaitOne();

            var results = await _database.SpeedTestResults
                .AsQueryable()
                .OrderByDescending(x => x.Timestamp)
                .Where(x => x.Timestamp >= start)
                .ToListAsync();

            Lock.ReleaseMutex();

            return results;
        }
    }
}