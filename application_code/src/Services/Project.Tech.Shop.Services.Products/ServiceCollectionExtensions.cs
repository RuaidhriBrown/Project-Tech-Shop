using System;
using Ardalis.GuardClauses;
using Project.Tech.Shop.Services.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Project.Tech.Shop.Services.Products.Repositories;

namespace Project.Tech.Shop.Services.Products
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all services to the provided <see cref="ServiceCollection"/> made available for the transactions service.
        /// </summary>
        /// <param name="services">The collection of services to register against.</param>
        /// <param name="options">A callback for configuration of the EF database context.</param>
        public static IServiceCollection AddProductServices(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
        {
            Guard.Against.Null(services, "Services is null");
            Guard.Against.Null(options, "options is null");

            services.AddDbContextFactory<ProductsContext>(options);
            services.AddSingleton<IDbContextFactorySource<ProductsContext>, CustomDbContextFactorySource<ProductsContext>>();
            services.AddScoped<IProductsRepository, ProductsRepository>();
            //services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<ISalesRepository, SalesRepository>();
            //services.AddScoped<ISalesService, SalesService>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            //services.AddScoped<IBasketService, BasketService>();

            return services;
        }
    }
}
