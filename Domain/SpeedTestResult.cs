using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Domain
{
    public class SpeedTestResult
    {
        protected SpeedTestResult()
        {
        }

        protected SpeedTestResult(decimal ping, decimal download, decimal upload)
        {
            Ping = ping;
            Download = download;
            Upload = upload;
        }

        [Key]
        public int SpeedTestResultId { get; protected set; }

        public DateTime Timestamp { get; set; }
        public decimal Ping { get; protected set; }
        public decimal Download { get; protected set; }
        public decimal Upload { get; protected set; }

        public static SpeedTestResult Parse(TextReader output)
        {
            var ping = Parse(output.ReadLine());
            var download = Parse(output.ReadLine());
            var upload = Parse(output.ReadLine());

            return new SpeedTestResult(ping, download, upload);
        }

        private static decimal Parse(string line)
        {
            return decimal.TryParse(line.Split(' ')[1], out decimal val) 
                ? val : 0;
        }
    }
}