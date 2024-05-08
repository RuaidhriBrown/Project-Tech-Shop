using Microsoft.EntityFrameworkCore;
using Prometheus;
using block.chain.webhost.Infastructure;
using Project.Tech.Shop.Web.Infastructure;
using Serilog;
using Project.Tech.Shop.Web.Infastructure.Options;

public class Startup
{
    private readonly SystemConfiguration _systemConfiguration;
    private string SystemDisplayName => $"{_systemConfiguration.SystemName} - {_systemConfiguration.SubSystemName}";
    private string VersionDisplayName => _systemConfiguration.VersionTag;

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;

#pragma warning disable CS8601 // Possible null reference assignment.
        _systemConfiguration = Configuration
            .GetSection(nameof(SystemConfiguration))
            .Get<SystemConfiguration>();
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();

        // Configure OpenTelemetry
        services.AddTelemetry(SystemDisplayName, VersionDisplayName)
                .AddAllHealthChecks()
                .AddServiceAppMetrics();

        // Configure DbContext options
        Action<DbContextOptionsBuilder> dbContextOptions = options =>
        {
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
        };

        // Add domain services
        services.AddDomainServices(dbContextOptions, Configuration, Environment);

        // Add cookie authentication
        services.AddAuthentication("CookieAuth")
            .AddCookie("CookieAuth", config =>
            {
                config.Cookie.Name = "UserLoginCookie"; // Set the cookie name
                config.LoginPath = "/Account/Login";    // Set the login path
            });

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        if (env.IsEnvironment("Testing"))
        {
            bool disableHttpsRedirection = Configuration.GetValue<bool>("DisableHttpsRedirection");
            if (!disableHttpsRedirection)
            {
                app.UseHttpsRedirection();
            }
        }
        else
        {
            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
        }

        
        app.UseStaticFiles();

        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseServiceAppMetrics();

        app.UseStaticFiles();

        // Prometheus metrics endpoint
        app.UseHttpMetrics();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthCheckEndpoint();
            endpoints.MapMetrics();  // Correct placement of MapMetrics

            endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
