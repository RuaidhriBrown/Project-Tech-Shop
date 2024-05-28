using System.Diagnostics;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Project.Tech.Shop.Web.Test.Playwright
{
    [SetUpFixture]
    public class GlobalSetup
    {
        private Process _webServerProcess;
        private string projectDirectory1 = @"..\..\..\src\Host Applications\Project.Tech.Shop.Web";
        private string projectDirectory = @"C:\Users\rbrow\source\repos\RuaidhriBrown\Project-Tech-Shop\application_code\src\Host Applications\Project.Tech.Shop.Web";
        //"\application_code\test\Host Applications\Project.Tech.Shop.Web.Test.Playwright"

        [OneTimeSetUp]
        public async Task StartWebApplication()
        {
            // First, attempt to stop any previously running instances
            await EnsureProcessStopped();

            _webServerProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "run --urls=http://localhost:5001",
                    WorkingDirectory = projectDirectory,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
            _webServerProcess.StartInfo.EnvironmentVariables["RUNNING_IN_TEST"] = "1";

            _webServerProcess.Start();
            await Task.Delay(5000); // Wait for the server to fully start
        }

        private async Task EnsureProcessStopped()
        {
            var processes = Process.GetProcessesByName("dotnet").Where(p => p.MainModule.FileName.StartsWith(projectDirectory));
            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                    process.WaitForExit(5000); // Ensure the process exits
                }
                catch
                {
                    // Possibly log this exception
                }
            }
        }


        [OneTimeTearDown]
        public async Task StopWebApplicationAsync()
        {
            if (_webServerProcess != null && !_webServerProcess.HasExited)
            {
                _webServerProcess.Kill(true);
                _webServerProcess.WaitForExit(5000);
                _webServerProcess.Dispose();
                _webServerProcess = null;
            }
        }
    }
}
