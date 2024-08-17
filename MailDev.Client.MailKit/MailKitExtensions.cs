using MailKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MailDev.Client.MailKit;

public static class MailKitExtensions
{
    public static void AddMailKitClient(this IHostApplicationBuilder builder, string connectionName,
        Action<MailKitClientSettings>? configureSettings = null)
    {
        AddMailKitClient(builder, MailKitClientSettings.DefaultConfigSectionName, configureSettings,
            connectionName, serviceKey: null);
    }

    public static void AddKeyedMailKitClient(this IHostApplicationBuilder builder, string name,
        Action<MailKitClientSettings>? configureSettings = null)
    {
        ArgumentNullException.ThrowIfNull(name);
        AddMailKitClient(builder, $"{MailKitClientSettings.DefaultConfigSectionName}:{name}",
            configureSettings, connectionName: name, serviceKey: name);
    }

    private static void AddMailKitClient(
        this IHostApplicationBuilder builder,
        string configurationSectionName,
        Action<MailKitClientSettings>? configureSettings,
        string connectionName,
        object? serviceKey)
    {
        #region Inject
        ArgumentNullException.ThrowIfNull(builder);

        var settings = new MailKitClientSettings();

        builder.Configuration
               .GetSection(configurationSectionName)
               .Bind(settings);

        if (builder.Configuration.GetConnectionString(connectionName) is string connectionString)
            settings.ParseConnectionString(connectionString);

        configureSettings?.Invoke(settings);

        if (serviceKey is null)
            builder.Services.AddScoped(_ => new MailKitClientFactory(settings));
        else
            builder.Services.AddKeyedScoped(serviceKey, (_, _) => new MailKitClientFactory(settings));
        #endregion

        #region Configure
        if (settings.DisableHealthChecks is false)
            builder.Services.AddHealthChecks()
                .AddCheck<MailKitHealthCheck>(
                    name: serviceKey is null ? "MailKit" : $"MailKit_{connectionName}",
                    failureStatus: default,
                    tags: []);

        if (settings.DisableTracing is false)
            builder.Services.AddOpenTelemetry()
                .WithTracing(traceBuilder => traceBuilder.AddSource(Telemetry.SmtpClient.ActivitySourceName));

        if (settings.DisableMetrics is false)
        {
            Telemetry.SmtpClient.Configure(); // Required by MailKit to enable metrics
            builder.Services.AddOpenTelemetry()
                .WithMetrics(metricsBuilder => metricsBuilder.AddMeter(Telemetry.SmtpClient.MeterName));
        }
        #endregion
    }
}