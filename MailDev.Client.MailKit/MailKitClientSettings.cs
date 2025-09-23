using System.Data.Common;

namespace MailDev.Client.MailKit;

public sealed class MailKitClientSettings
{
    public bool DisableHealthChecks { get; set; }

    public bool DisableMetrics { get; set; }

    public bool DisableTracing { get; set; }

    public Uri? Endpoint { get; set; }

    internal void ParseConnectionString(
        string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"""
                 ConnectionString is missing.
                 It should be provided in 'ConnectionStrings:<connectionName>'
                 or '{DefaultConfigSectionName}:Endpoint' key.'
                 configuration section.
                 """);
        }

        if (Uri.TryCreate(
                uriString: connectionString,
                uriKind: UriKind.Absolute,
                result: out Uri? strUri))
        {
            Endpoint = strUri;
        }
        else
        {
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            if (builder.TryGetValue(
                    keyword: "Endpoint",
                    value: out object? endpoint) is false)
            {
                throw new InvalidOperationException(
                    $"""
                     The 'ConnectionStrings:<connectionName>' (or 'Endpoint' key in
                     '{DefaultConfigSectionName}') is missing.
                     """);
            }

            if (Uri.TryCreate(
                    uriString: endpoint.ToString(),
                    uriKind: UriKind.Absolute,
                    result: out Uri? uri) is false)
            {
                throw new InvalidOperationException(
                    $"""
                     The 'ConnectionStrings:<connectionName>' (or 'Endpoint' key in
                     '{DefaultConfigSectionName}') isn't a valid URI.
                     """);
            }

            Endpoint = uri;
        }
    }
    internal const string DefaultConfigSectionName = "MailKit:Client";
}
