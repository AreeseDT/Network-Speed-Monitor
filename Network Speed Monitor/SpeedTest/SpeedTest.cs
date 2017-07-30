using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace NetworkSpeedMonitor.SpeedTest
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
                    FileName = "./Tools/speedtest-cli",
                    Arguments = "--server 6989 --simple",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
        }

        public async Task<SpeedTestResult> Run()
        {
            _process.Start();
            _process.WaitForExit();

            return await SpeedTestResult.Parse(_process.StandardOutput);
        }
    }
}