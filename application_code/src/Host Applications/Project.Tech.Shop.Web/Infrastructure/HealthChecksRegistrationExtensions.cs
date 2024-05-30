using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Project.Tech.Shop.Web.Infrastructure
{
    /// <summary>
    /// Provides extension methods for registering the required services within a given <see cref="IServiceCollection"/>.
    /// </summary>
    public static class HealthChecksRegistrationExtensions
    {
        /// <summary>
        /// Adds the required services for web application health checks and registers all pertinent health check implementations.
        /// </summary>
        /// <param name="services">The service collection with which services should be registered.</param>
        /// <returns>The provided <paramref name="services"/>.</returns>
        public static IServiceCollection AddAllHealthChecks(this IServiceCollection services)
        {
            services
                .AddHealthChecks();
            // TODO - any other health checks

            return services;
        }

        /// <summary>
        /// Maps the "/health" endpoint as the endpoint to call for health checks.
        /// </summary>
        /// <param name="builder">The endpoint builder.</param>
        /// <returns>The endpoint builder.</returns>
        public static IEndpointRouteBuilder MapHealthCheckEndpoint(this IEndpointRouteBuilder builder)
        {
            Func<HttpContext, HealthReport, Task> healthReportBodyWriter = async (context, report) =>
            {
                await context.Response.WriteAsJsonAsync(report);
            };
            builder.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = healthReportBodyWriter });
            return builder;
        }
    }
}
