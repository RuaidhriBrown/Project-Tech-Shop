using Microsoft.Playwright;
using AventStack.ExtentReports;
using System.Threading.Tasks;

namespace Project.Tech.Shop.Web.Test.Playwright.Pages
{
    public class RegisterPage
    {
        private readonly IPage _page;
        private readonly ExtentTest _test;

        // Define selectors as private readonly fields
        private readonly string _usernameSelector = "input[name='Username']";
        private readonly string _emailSelector = "input[name='Email']";
        private readonly string _firstNameSelector = "input[name='FirstName']";
        private readonly string _lastNameSelector = "input[name='LastName']";
        private readonly string _passwordSelector = "input[name='Password']";
        private readonly string _confirmPasswordSelector = "input[name='ConfirmPassword']";
        private readonly string _submitSelector = "button[type='submit']";

        public RegisterPage(IPage page, ExtentTest test)
        {
            _page = page;
            _test = test;
        }

        public async Task FillRegistrationFormAsync(string username, string email, string firstName, string lastName, string password, string confirmPassword)
        {
            _test.Log(Status.Info, "Filling the registration form.");
            await _page.FillAsync(_usernameSelector, username);
            await _page.FillAsync(_emailSelector, email);
            await _page.FillAsync(_firstNameSelector, firstName);
            await _page.FillAsync(_lastNameSelector, lastName);
            await _page.FillAsync(_passwordSelector, password);
            await _page.FillAsync(_confirmPasswordSelector, confirmPassword);
        }

        public async Task SubmitRegistrationAsync()
        {
            _test.Log(Status.Info, "Submitting the registration form.");
            await _page.ClickAsync(_submitSelector);
        }

        // Additional methods for validation or interaction can also use these selectors
    }
}
