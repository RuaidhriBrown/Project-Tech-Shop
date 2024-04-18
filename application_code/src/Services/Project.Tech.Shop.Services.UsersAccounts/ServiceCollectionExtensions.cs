using System;
using Ardalis.GuardClauses;
using Project.Tech.Shop.Services.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using block.chain.services.Transactions.Repositories;

namespace Project.Tech.Shop.Services.UsersAccounts
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all services to the provided <see cref="ServiceCollection"/> made available for the transactions service.
        /// </summary>
        /// <param name="services">The collection of services to register against.</param>
        /// <param name="options">A callback for configuration of the EF database context.</param>
        public static IServiceCollection AddUserAccountsServices(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
        {
            Guard.Against.Null(services, "Services is null");
            Guard.Against.Null(options, "options is null");

            services.AddDbContextFactory<UserAccountsContext>(options);
            services.AddSingleton<IDbContextFactorySource<UserAccountsContext>, CustomDbContextFactorySource<UserAccountsContext>>();
            services.AddScoped<IUserAccountsRepository, UserAccountsRepository>();
            //services.AddScoped<IUserAccountsService, UserAccountsService>();

            return services;
        }
    }
}
