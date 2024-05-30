using OpenTelemetry.Trace;
using System.Diagnostics;
using OpenTelemetry.Resources;

namespace Project.Tech.Shop.Web.Infrastructure;

public static class TelemetryExtensions
{
    public static IServiceCollection AddTelemetry(
        this IServiceCollection services,
        string serviceName,
        string serviceVersion,
        Action<TracerProviderBuilder>? callback = null)
    {
        Debug.Assert(serviceName != null);

        services
            .AddOpenTelemetry()
            .ConfigureResource(x => x.AddService(serviceName: serviceName, serviceVersion: serviceVersion))
            .WithTracing(x =>
            {
                x.AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddZipkinExporter();

                callback?.Invoke(x);
            });

        return services;
    }
}