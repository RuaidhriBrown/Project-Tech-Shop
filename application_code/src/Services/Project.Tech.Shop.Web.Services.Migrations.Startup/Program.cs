using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Project.Tech.Shop.Services.Products;
using Project.Tech.Shop.Services.UsersAccounts;
using System;
using static CSharpFunctionalExtensions.Result;

var builder = Host.CreateDefaultBuilder(args);

// Add configuration files based on the environment
builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    var env = hostingContext.HostingEnvironment;

    // Load the basic configuration file
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

    // Load environment-specific configuration file
    if (env.IsDevelopment() || environmentName == "Development")
    {
        config.AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
    }

    // This could be extended to other environments like staging or production if needed
});

// Configure services
builder.ConfigureServices((hostContext, services) =>
{
    IConfiguration configuration = hostContext.Configuration;
    Console.WriteLine($"Using connection string: {configuration.GetConnectionString("DefaultConnection")}");
    services.AddDbContext<UserAccountsContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    services.AddDbContext<ProductsContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
});

var app = builder.Build();

// Applying migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var dbContextUsers = services.GetRequiredService<UserAccountsContext>();
        Console.WriteLine("Applying migrations for UserAccounts...");
        dbContextUsers.Database.Migrate();
        Console.WriteLine("Migrations for UserAccounts applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Failed to apply migrations for UserAccounts: " + ex.Message);
    }

    try
    {
        var dbContextProducts = services.GetRequiredService<ProductsContext>();
        Console.WriteLine("Applying migrations for Products...");
        dbContextProducts.Database.Migrate();
        Console.WriteLine("Migrations for Products applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Failed to apply migrations for Products: " + ex.Message);
    }
}

app.Run();
