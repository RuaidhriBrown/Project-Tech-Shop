using AventStack.ExtentReports;
using Microsoft.Playwright;
using NUnit.Framework;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Project.Tech.Shop.Web.Test.Playwright
{
    public abstract class PlaywrightTestsBase
    {
        protected IPlaywright _playwright;
        protected IBrowser _browser;
        protected IBrowserContext _context;
        protected IPage _page;

        //protected string baseUrl = "http://localhost:5001/";
        protected string baseUrl = "http://localhost:5045/";
        private BrowserTypeLaunchOptions SetBrowserTypeLaunchOptions = new BrowserTypeLaunchOptions
        {
            Headless = false
        };

        private string _browserType;
        protected ExtentTest test;

        private string? TestName;

        public PlaywrightTestsBase(string browserType)
        {
            _browserType = browserType;
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            // Initialize Playwright and open a page
            _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            _browser = await LaunchBrowserAsync(_browserType);

            // Generate a unique directory path for each test to store the video
            var dateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var testName = $"{TestContext.CurrentContext.Test.Name}_[{_browserType}]_{dateTime}\"";
            TestName = SanitizeForPath(testName);
            var testVideoPath = Path.Combine(ReportManager.getReportDirectory(), "Videos", TestName);
            Directory.CreateDirectory(testVideoPath);

            // Setup the browser context with video recording options
            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                RecordVideoDir = testVideoPath,
                RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
            });

            _page = await _context.NewPageAsync();
            await _page.GotoAsync(baseUrl);
        }

        [SetUp]
        public void StartTest()
        {
            test = ReportManager.Instance.CreateTest($"{TestContext.CurrentContext.Test.Name}_[{_browserType}]");
            test.AssignCategory(_browserType);
            test.AssignCategory(GetOperatingSystemCategory());
            test.Info($"Test started at: {DateTime.Now:HH:mm:ss}");
        }

        [TearDown]
        public async Task EndTest()
        {
            var result = TestContext.CurrentContext.Result;
            string screenshotPath = await CaptureScreenshotAsync(TestContext.CurrentContext.Test.Name);

            test.Info($"Test ended at: {DateTime.Now:HH:mm:ss}");

            switch (result.Outcome.Status)
            {
                case NUnit.Framework.Interfaces.TestStatus.Failed:
                    test.Fail(result.Message).Fail("<pre>" + result.StackTrace + "</pre>").AddScreenCaptureFromPath(screenshotPath);
                    break;
                case NUnit.Framework.Interfaces.TestStatus.Passed:
                    test.Pass("Test passed").AddScreenCaptureFromPath(screenshotPath);
                    break;
                case NUnit.Framework.Interfaces.TestStatus.Skipped:
                    test.Skip("Test skipped");
                    break;
                default:
                    test.Info("Test finished with an unexpected status.");
                    break;
            }

            // Optionally add a link to the video file in the report
            string videoPath = await GetLatestVideoPath();
            if (!string.IsNullOrEmpty(videoPath))
            {
                test.Info($"Video recording: <a href='file:///{videoPath}' target='_blank'>Click here to view</a>");
            }
            else
            {
                test.Log(Status.Info, "No video was recorded for this test.");
            }
        }

        private async Task<IBrowser> LaunchBrowserAsync(string browserType)
        {
            switch (browserType.ToLower())
            {
                case "firefox":
                    return await _playwright.Firefox.LaunchAsync(SetBrowserTypeLaunchOptions);
                case "webkit":
                    return await _playwright.Webkit.LaunchAsync(SetBrowserTypeLaunchOptions);
                default:
                    return await _playwright.Chromium.LaunchAsync(SetBrowserTypeLaunchOptions);
            }
        }

        [OneTimeTearDown]
        public async Task FinalizeReport()
        {
            await _context.CloseAsync();
            await _browser.CloseAsync();
            ReportManager.FlushReports();  // Flush and close the report at the end of tests
        }

        private async Task<string> CaptureScreenshotAsync(string testName)
        {
            var screenshotsDirectory = Path.Combine(ReportManager.getReportDirectory(), "Screenshots"); ;
            Directory.CreateDirectory(screenshotsDirectory);

            string screenshotPath = Path.Combine(screenshotsDirectory, $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
            return screenshotPath;
        }

        private string GetOperatingSystemCategory()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "Windows";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "macOS";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "Linux";
            else
                return "Unknown OS";
        }

        private string SanitizeForPath(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var validName = new string(name.Where(ch => !invalidChars.Contains(ch)).ToArray());
            return validName.Replace(" ", "_"); // Replace spaces with underscores for better readability
        }

        private async Task<string> GetLatestVideoPath()
        {
            string videosPath = Path.Combine(ReportManager.getReportDirectory(), "Videos", TestName);
            var directoryInfo = new DirectoryInfo(videosPath);
            var videoFile = directoryInfo.GetFiles("*.webm").OrderByDescending(f => f.CreationTime).FirstOrDefault();
            return videoFile?.FullName;
        }
    }
}
