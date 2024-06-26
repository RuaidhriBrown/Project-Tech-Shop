﻿using Microsoft.EntityFrameworkCore;
using Project.Tech.Shop.Services.UsersAccounts;
using Ardalis.GuardClauses;
using Project.Tech.Shop.Web.services;
using Project.Tech.Shop.Services.Products;
using Project.Tech.Shop.Web.Infrastructure.Filter;

namespace Project.Tech.Shop.Web.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbContextOptions"></param>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static IServiceCollection AddDomainServices(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> dbContextOptions,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            services.AddUserAccountsServices(dbContextOptions);
            services.AddProductServices(dbContextOptions);
            services.AddOtherServices(configuration, dbContextOptions);
            return services;
        }


        /// <summary>
        /// Adds all services to the provided <see cref="ServiceCollection"/> made available for the web app.
        /// </summary>
        /// <param name="services">The collection of services to register against.</param>
        /// <param name="configuration">An <see cref="IConfiguration"/> representing the application configuration settings.</param>
        /// <param name="options">A callback for configuration of the EF database context.</param>
        public static IServiceCollection AddOtherServices(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<DbContextOptionsBuilder> options)
        {
            Guard.Against.Null(services, "");
            Guard.Against.Null(options, "");

            // UseCases
            services.AddScoped<GetUserProfileUseCase>();
            services.AddScoped<AccountUseCase>();
            services.AddScoped<AdminAuthorizationFilter>();

            return services;
        }
    }
}
