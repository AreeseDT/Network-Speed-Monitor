using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    public class Database : DbContext
    {
        public DbSet<SpeedTestResult> SpeedTestResults { get; set; }

        public IQueryable<SpeedTestResult> MostRecentResults(DateTime after, int max)
        {
            var query = SpeedTestResults
                .AsQueryable()
                .OrderByDescending(x => x.Timestamp)
                .Where(x => x.Timestamp >= after);

            return query.Count() > max
                ? query.Take(max)
                : query;
        } 
            

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=InternetMonitor.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<SpeedTestResult>();
            entity.Property(e => e.Ping).ForSqliteHasColumnType("decimal(8,3)");
            entity.Property(e => e.Upload).ForSqliteHasColumnType("decimal(8,3)");
            entity.Property(e => e.Download).ForSqliteHasColumnType("decimal(8,3)");

            entity.HasIndex(e => e.Timestamp);
        }
    }
}