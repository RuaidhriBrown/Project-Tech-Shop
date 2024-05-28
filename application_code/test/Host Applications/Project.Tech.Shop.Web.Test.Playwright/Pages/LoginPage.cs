using AventStack.ExtentReports;
using Microsoft.Playwright;

namespace Project.Tech.Shop.Web.Test.Playwright.Pages
{
    public class LoginPage
    {
        private readonly IPage _page;
        private readonly ExtentTest _test;

        public LoginPage(IPage page, ExtentTest test)
        {
            _page = page;
            _test = test;
        }

        public async Task NavigateToLoginByLoginLinkAsync()
        {
            _test.Log(Status.Info, "Navigating to the login page.");

            // Click on the Login link to navigate to the login page
            await _page.ClickAsync("text=Login");

            _test.Log(Status.Info, "Navigated to the login page.");
        }

        public async Task LoginAsync(string username, string password)
        {
            await _page.FillAsync("input[name='Username']", username);
            await _page.FillAsync("input[name='Password']", password);
            _test.Log(Status.Info, $"Entered Username: {username} and Password: [protected]");

            await _page.ClickAsync("input[type='submit'][value='Log in']");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle); // Ensures the page has loaded
            _test.Log(Status.Info, "Completed login attempt");
        }

        public async Task NaviagateAndLoginAsync(string username, string password)
        {
            await NavigateToLoginByLoginLinkAsync();

            await LoginAsync(username, password);
        }

        public async Task<RegisterPage> GoToRegisterPageAsync()
        {
            await NavigateToLoginByLoginLinkAsync();

            _test.Log(Status.Info, "Navigating to the registration page.");
            await _page.ClickAsync("text=Register here");
            return new RegisterPage(_page, _test);
        }


        public async Task<bool> IsUsernameFieldVisible()
        {
            return await _page.Locator("input[name='Username']").IsVisibleAsync();
        }

        public async Task<bool> IsPasswordFieldVisible()
        {
            return await _page.Locator("input[name='Password']").IsVisibleAsync();
        }

        public async Task<bool> IsLoginButtonVisible()
        {
            return await _page.Locator("input[type='submit'][value='Log in']").IsVisibleAsync();
        }
    }

}
