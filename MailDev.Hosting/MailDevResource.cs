using Aspire.Hosting.ApplicationModel;

namespace MailDev.Hosting;

public sealed class MailDevResource(
    string name) : ContainerResource(name),
    IResourceWithConnectionString
{
    private EndpointReference? _smtpReference;

    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"smtp://{SmtpEndpoint.Property(EndpointProperty.Host)}:{SmtpEndpoint.Property(EndpointProperty.Port)}");

    public EndpointReference SmtpEndpoint => _smtpReference ??= new EndpointReference(
        owner: this,
        endpointName: SmtpEndpointName);

    internal const string HttpEndpointName = "http";
    internal const string SmtpEndpointName = "smtp";
}
