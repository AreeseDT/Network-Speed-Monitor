using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;

namespace NetworkSpeedMonitor
{
    public class CsvLogger<T>
    {
        public string LogFile { get; }

        public CsvLogger(string logFile)
        {
            if (string.IsNullOrWhiteSpace(logFile))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(logFile));

            LogFile = logFile;
        }

        public void Append(T value)
        {
            using (var writer = File.AppendText(LogFile))
            using (var csv = new CsvWriter(writer))
            {
                if (!File.Exists(LogFile))
                    csv.WriteHeader(typeof(T));

                csv.WriteRecord(value);
            }
        }

        public static void WriteFile(IEnumerable<T> values, string file)
        {
            using (var stream = File.OpenWrite(file))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteHeader(typeof(T));
                csv.WriteRecords(values);
            }
        }
    }
}