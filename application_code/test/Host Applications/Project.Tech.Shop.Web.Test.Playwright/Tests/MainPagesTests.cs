using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
using AventStack.ExtentReports;

namespace Project.Tech.Shop.Web.Test.Playwright.Tests
{
    [TestFixture("chromium")]
    [TestFixture("firefox")]
    [TestFixture("webkit")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class MainPagesTests : PlaywrightTestsBase
    {
        public MainPagesTests(string browserType) : base(browserType)
        {
        }

        [Test]
        public async Task HomePageShouldExist()
        {
            await _page.GotoAsync(baseUrl);
            var content = await _page.ContentAsync();
            Assert.That(content, Is.Not.Empty);

            var navbarExists = await _page.Locator("nav.navbar").IsVisibleAsync();
            Assert.IsTrue(navbarExists, "Navbar should be visible on the home page.");

            // Example: Check if the footer is present
            var footerExists = await _page.Locator("footer.footer").IsVisibleAsync();
            Assert.IsTrue(footerExists, "Footer should be visible on the home page.");

            // Example: Check for the presence of the login link if not authenticated
            var loginLinkExists = await _page.Locator("a.nav-link:has-text('Login')").IsVisibleAsync();
            Assert.IsTrue(loginLinkExists, "Login link should be visible on the home page when not logged in.");

            // Optional: Check for text content in specific elements
            var mainHeadingText = await _page.Locator("h1").TextContentAsync();
            Assert.IsNotNull(mainHeadingText, "Main heading should have text.");
        }

        [Test]
        public async Task LoginPageShouldExist()
        {
            await _page.GotoAsync(baseUrl + "Account/Login");

            var usernameFieldExists = await _page.Locator("input[name='Username']").IsVisibleAsync();
            Assert.IsTrue(usernameFieldExists, "Username input field should be visible on the login page.");

            var passwordFieldExists = await _page.Locator("input[name='Password']").IsVisibleAsync();
            Assert.IsTrue(passwordFieldExists, "Password input field should be visible on the login page.");

            var loginButtonExists = await _page.Locator("input[type='submit'][value='Log in']").IsVisibleAsync();
            Assert.IsTrue(loginButtonExists, "Login button should be visible on the login page.");
        }


        [Test]
        public async Task CrazyPageShouldExist()
        {
            await _page.GotoAsync(baseUrl + "Crazy/SimpleGame");

            var content = await _page.ContentAsync();
            Assert.That(content, Is.Not.Empty);
        }

        [Test]
        public async Task ProductsPageShouldShouldExist()
        {
            await _page.GotoAsync(baseUrl + "Products");

            var content = await _page.ContentAsync();
            Assert.That(content, Is.Not.Empty);
        }
    }
}
