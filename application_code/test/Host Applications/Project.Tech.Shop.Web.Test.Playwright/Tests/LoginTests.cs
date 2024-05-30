using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
using AventStack.ExtentReports;
using Project.Tech.Shop.Web.Test.Playwright.Pages;

namespace Project.Tech.Shop.Web.Test.Playwright.Tests
{
    [TestFixture("chromium")]
    [TestFixture("firefox")]
    [TestFixture("webkit")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class LoginTests : PlaywrightTestsBase
    {
        private readonly string _username;
        private const string Password = "Project1";

        public LoginTests(string browserType) : base(browserType)
        {
            _username = browserType switch
            {
                "chromium" => "chromiumUser",
                "firefox" => "firefoxUser",
                "webkit" => "webkitUser",
                _ => throw new ArgumentException("Invalid browser type")
            };
        }

        [Test]
        public async Task LoginPageBeNavigatableTo()
        {
            LoginPage loginPage = new LoginPage(_page, test);

            // Navigate to login
            await loginPage.NavigateToLoginByLoginLinkAsync();

            // Check if Username field is present
            Assert.IsTrue(await loginPage.IsUsernameFieldVisible(), "Username input field should be visible on the login page.");

            // Check if Password field is present
            Assert.IsTrue(await loginPage.IsPasswordFieldVisible(), "Password input field should be visible on the login page.");

            // Check if the login button is present
            Assert.IsTrue(await loginPage.IsLoginButtonVisible(), "Login button should be visible on the login page.");

            test.Log(Status.Info, "Test completed successfully.");
        }

        [Test]
        public async Task LoginPageShouldLogin()
        {
            LoginPage loginPage = new LoginPage(_page, test);

            // Navigate to login
            await loginPage.NavigateToLoginByLoginLinkAsync();

            // Check if Username field is present
            Assert.IsTrue(await loginPage.IsUsernameFieldVisible(), "Username input field should be visible on the login page.");

            // Check if Password field is present
            Assert.IsTrue(await loginPage.IsPasswordFieldVisible(), "Password input field should be visible on the login page.");

            // Check if the login button is present
            Assert.IsTrue(await loginPage.IsLoginButtonVisible(), "Login button should be visible on the login page.");

            // Perform login
            await loginPage.LoginAsync(_username, Password);

            // Find login evidence
            var usernameDisplay = await _page.Locator("#navbarAccountDropdownMenuLink").TextContentAsync();
            bool isLoginSuccessful = !string.IsNullOrEmpty(usernameDisplay) && usernameDisplay != "Login";

            Assert.IsTrue(isLoginSuccessful, "Login should be successful.");

            test.Log(Status.Info, "Test completed successfully.");
        }

        [Test]
        public async Task LoginPageShouldHandleFailedLogin()
        {
            LoginPage loginPage = new LoginPage(_page, test);

            // Navigate to login
            await loginPage.NavigateToLoginByLoginLinkAsync();

            // Ensure necessary elements are visible
            Assert.IsTrue(await loginPage.IsUsernameFieldVisible(), "Username input field should be visible.");
            Assert.IsTrue(await loginPage.IsPasswordFieldVisible(), "Password input field should be visible.");
            Assert.IsTrue(await loginPage.IsLoginButtonVisible(), "Login button should be visible.");

            // Perform login with incorrect credentials
            await loginPage.LoginAsync("wronguser", "wrongpassword");

            // Check for error message visibility
            var errorVisible = await _page.Locator(".text-danger:visible").IsVisibleAsync();
            Assert.IsTrue(errorVisible, "Error message should be visible after failed login.");

            test.Log(Status.Info, "Test completed successfully.");
        }

        [Test]
        public async Task LoginPageShouldPreventSQLInjection()
        {
            LoginPage loginPage = new LoginPage(_page, test);

            // Navigate to login
            await loginPage.NavigateToLoginByLoginLinkAsync();

            // Ensure necessary elements are visible
            Assert.IsTrue(await loginPage.IsUsernameFieldVisible(), "Username input field should be visible.");
            Assert.IsTrue(await loginPage.IsPasswordFieldVisible(), "Password input field should be visible.");
            Assert.IsTrue(await loginPage.IsLoginButtonVisible(), "Login button should be visible.");

            // Perform login attempt with SQL injection
            string sqlInjectionString = "' OR '1'='1";
            await loginPage.LoginAsync(sqlInjectionString, sqlInjectionString);

            // Check for error message or failed login indication
            var errorVisible = await _page.Locator(".validation-summary-errors:visible").IsVisibleAsync();
            Assert.IsTrue(errorVisible, "Error message should be visible after SQL injection attempt.");

            test.Log(Status.Info, "Test completed successfully.");
        }

    }

}
