using Project.Tech.Shop.Web.Test.Playwright.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tech.Shop.Web.Test.Playwright.Tests
{
    [TestFixture("chromium")]
    [TestFixture("firefox")]
    [TestFixture("webkit")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class RegistrationTests : PlaywrightTestsBase
    {
        public RegistrationTests(string browserType) : base(browserType) { }

        [Test]
        public async Task RegistrationProcessShouldBeSuccessful()
        {
            LoginPage loginPage = new LoginPage(_page, test);
            RegisterPage registerPage = await loginPage.GoToRegisterPageAsync();

            await registerPage.FillRegistrationFormAsync(
                "newuser1",
                "newuser@example.com",
                "First",
                "Last",
                "password123",
                "password123"
            );

            await registerPage.SubmitRegistrationAsync();

            // Additional assertions can be made here to verify registration success
        }
    }
}
