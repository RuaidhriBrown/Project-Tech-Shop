using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tech.Shop.Web.Test.Playwright
{
    public static class ReportManager
    {
        private static ExtentReports _extent;
        private static string _reportFilePath = SetupReportPath();

        public static ExtentReports Instance
        {
            get
            {
                if (_extent == null)
                {
                    var reporter = new ExtentSparkReporter(_reportFilePath);
                    _extent = new ExtentReports();
                    _extent.AttachReporter(reporter);
                }
                return _extent;
            }
        }

        public static string getReportDirectory()
        {
            return(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Reports"));
        }

        private static string SetupReportPath()
        {
            var reportDirectory = getReportDirectory();
            if (!Directory.Exists(reportDirectory))
            {
                Directory.CreateDirectory(reportDirectory);
            }
            return Path.Combine(reportDirectory, $"ExtentReport-{DateTime.Now:yyyyMMddHHmmss}.html");
        }

        public static void FlushReports() => _extent?.Flush();
    }

}
