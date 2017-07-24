using System.Diagnostics;
using System.Threading;
using Domain;

namespace NetworkSpeedMonitor
{
    public class SpeedTest
    {
        private readonly Process _process;

        public SpeedTest()
        {
            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "./speedtest-cli",
                    Arguments = "--server 6989 --simple",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
        }

        public SpeedTestResult Run(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => _process.Close());

            _process.Start();
            _process.WaitForExit();

            return SpeedTestResult.Parse(_process.StandardOutput);
        }
    }
}