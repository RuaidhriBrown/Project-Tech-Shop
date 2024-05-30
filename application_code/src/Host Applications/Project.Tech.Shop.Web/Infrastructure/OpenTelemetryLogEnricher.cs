using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog;
using System.Diagnostics;

namespace Project.Tech.Shop.Web.Infrastructure
{
    public class OpenTelemetryLogEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (Activity.Current != null)
            {
                logEvent.AddPropertyIfAbsent(new LogEventProperty(nameof(Activity.Current.TraceId), new ScalarValue(Activity.Current.TraceId.ToString())));
                logEvent.AddPropertyIfAbsent(new LogEventProperty(nameof(Activity.Current.SpanId), new ScalarValue(Activity.Current.SpanId.ToString())));
                logEvent.AddPropertyIfAbsent(new LogEventProperty(nameof(Activity.Current.ParentId), new ScalarValue(Activity.Current.ParentId)));
                logEvent.AddPropertyIfAbsent(new LogEventProperty(nameof(Activity.Current.ParentSpanId), new ScalarValue(Activity.Current.ParentSpanId)));
                logEvent.AddPropertyIfAbsent(new LogEventProperty(nameof(Activity.Current.TraceStateString), new ScalarValue(Activity.Current.TraceStateString)));
            }
        }
    }

    /// <summary>
    /// This is the bit that gets used by the configuration to add the actual <see cref="OpenTelemetryLogEnricher"/>.
    /// </summary>
    public static class LoggingExtensions
    {
        public static LoggerConfiguration WithOpenTelemetry(this LoggerEnrichmentConfiguration enrich)
        {
            return enrich.With<OpenTelemetryLogEnricher>();
        }
    }
}
