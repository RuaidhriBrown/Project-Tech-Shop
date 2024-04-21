using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Project.Tech.Shop.Web.Infastructure
{
    public static class AppMetricsExtensions
    {
        /// <summary>
        /// Adds the required services for AppMetrics output that links to our Prometheus monitoring.
        /// </summary>
        /// <param name="services">The service collection into which services should be registered.</param>
        /// <returns>The provides service collection.</returns>
        public static IServiceCollection AddServiceAppMetrics(this IServiceCollection services)
        {
            return services
                .AddMetrics()
                .AddMetricsEndpoints()
                .AddMetricsTrackingMiddleware();
        }

        /// <summary>
        /// Adds middleware support for AppMetrics to output monitoring data suitable for consumption
        /// by Prometheus.
        /// </summary>
        /// <param name="app">The current application builder.</param>
        /// <returns>The supplied application builder.</returns>
        public static IApplicationBuilder UseServiceAppMetrics(this IApplicationBuilder app)
        {
            return app
                .UseMetricsEndpoint(new MetricsPrometheusProtobufOutputFormatter())
                .UseMetricsTextEndpoint(new MetricsPrometheusTextOutputFormatter())
                .UseMetricsAllMiddleware()
                .UseMetricsRouteBasedOnRequestPath();
        }

        /// <summary>
        /// <remarks>
        /// The AppMetrics implementation tags each request with a route name based on the resolved MVC action
        /// method via a <see cref="IResourceFilter"/> implementation. This only works if the pipeline reaches
        /// the MVC middleware, in cases of failure such as authentication issues, the MVC pipeline is never
        /// reached and so no action method matching occurs.
        /// </remarks>
        /// <remarks>
        /// This implementation instead sets the app metrics route name based on the request path so that
        /// we get a breakdown by URL path when authentication issues occur.
        /// </remarks>
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        private static IApplicationBuilder UseMetricsRouteBasedOnRequestPath(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path;

                if (!string.IsNullOrWhiteSpace(path))
                {
                    context.AddMetricsCurrentRouteName(path);
                }
                await next();
            });
            return app;
        }
    }
}
