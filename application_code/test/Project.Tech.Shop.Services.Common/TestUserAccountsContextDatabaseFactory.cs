using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Project.Tech.Shop.Services.UsersAccounts;
using Project.Tech.Shop.Services.UsersAccounts.Entities;

namespace Project.Tech.Shop.Tests.Common
{
    public static class TestUserAccountsContextDatabaseFactory
    {
        public static UserAccountsContext CreateDefaultTestContext()
        {
            var databaseOptions = TestDatabaseContextFactory<UserAccountsContext>.CreateDefaultTestContextOption();
            return new TestUserAccountsContext(databaseOptions);
        }

        public static async Task<User> CreateTestProduct(
            UserAccountsContext dbContext,
            Fixture fixture,
            bool save = true)
        {
            var userAccount = new User()
            {
                UserId = Guid.NewGuid(),
                Username = fixture.Create<string>(),
                FirstName = fixture.Create<string>(),
                Surname = fixture.Create<string>(),
                Email = fixture.Create<string>(),
                PasswordHash = fixture.Create<string>(),
                Role = fixture.Create<Role>(),
                Status = fixture.Create<AccountStatus>()
            };

            userAccount.Addresses = new List<Address>()
            {
                new Address()
                {
                    AddressId = Guid.NewGuid(),
                    UserId = userAccount.UserId,
                    User = userAccount,
                    AddressLine = fixture.Create<string>(),
                    City = fixture.Create<string>(),
                    County = fixture.Create<string>(),
                    PostCode = fixture.Create<string>(),
                    Country = fixture.Create<string>(),
                    IsShippingAddress = fixture.Create<bool>(),
                    IsBillingAddress = fixture.Create<bool>()
                }
            };

            userAccount.SecuritySettings = new SecuritySettings()
            {
                UserId = userAccount.UserId,
                User = userAccount,
                TwoFactorEnabled = fixture.Create<bool>(),
                SecurityQuestion = fixture.Create<string>(),
                SecurityAnswerHash = fixture.Create<string>()
            };

            userAccount.Preferences = new UserPreferences()
            {
                UserId = userAccount.UserId,
                User = userAccount,
                ReceiveNewsletter = fixture.Create<bool>(),
                PreferredPaymentMethod = fixture.Create<string>()
            };

            userAccount.Activities = new List<AccountActivity>()
            {
                new AccountActivity()
                {
                    ActivityId = Guid.NewGuid(),
                    UserId = userAccount.UserId,
                    User = userAccount,
                    Timestamp = DateTime.UtcNow,
                    ActivityType = fixture.Create<string>(),
                    Description = fixture.Create<string>()
                }
            };

            if (save)
            {
                dbContext.Users.Add(userAccount);
                await dbContext.SaveChangesAsync();
            }

            return userAccount;
        }

        private class TestUserAccountsContext : UserAccountsContext
        {
            public TestUserAccountsContext(DbContextOptions<UserAccountsContext> options) : base(options)
            {
            }

            public override void Dispose()
            {
            }

            public override ValueTask DisposeAsync()
            {
                return new ValueTask(Task.CompletedTask);
            }
        }

    }
}