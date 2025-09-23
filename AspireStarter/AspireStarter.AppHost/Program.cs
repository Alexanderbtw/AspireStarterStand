using MailDev.Hosting;
using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> parameter = builder.AddParameter(
    name: "parameterName",
    secret: false);

IResourceBuilder<ParameterResource> secretParameter = builder.AddParameter(
    name: "secretParameterName",
    secret: true);

IResourceBuilder<IResourceWithConnectionString> mssql = builder.AddConnectionString("mssql");

IResourceBuilder<RedisResource> cache = builder.AddRedis("cache");
IResourceBuilder<MailDevResource> mailDev = builder.AddMailDev("maildev");
IResourceBuilder<ProjectResource> apiService = builder
    .AddProject<AspireStarter_ApiService>("apiservice")
    .WithEnvironment(
        name: "MY_PARAMETER",
        parameter: parameter)
    .WithEnvironment(
        name: "MY_SECRET_PARAMETER",
        parameter: secretParameter)
    .WithReference(mssql);

IResourceBuilder<ProjectResource> frontend = builder
    .AddProject<AspireStarter_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService)
    .WithReference(mailDev);

builder
    .AddNpmApp(
        name: "reactfrontend",
        workingDirectory: "../../ReactFrontend/react-frontend",
        scriptName: builder.ExecutionContext.IsRunMode
            ? "dev"
            : "start") // Aspire.Hosting.NodeJs
    .WithReference(apiService)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

if (builder.ExecutionContext.IsPublishMode)
{
    // TODO: ELK Stack

    IResourceBuilder<ContainerResource> otelCollector = builder
        .AddContainer(
            name: "collector",
            image: "otel/opentelemetry-collector-contrib")
        .WithArgs(
            "--config",
            "/etc/otel-collector.yaml")

        // Volumes not working in Aspir8 when generate docker-compose
        .WithBindMount(
            source: "configs/otel-collector.yaml",
            target: "/etc/otel-collector.yaml",
            isReadOnly: true)
        .WithHttpEndpoint(
            port: 55679,
            targetPort: 55679,
            name: "zpages") // zpages
        .WithHttpEndpoint(
            port: 4317,
            targetPort: 4317) // gRPC
        .WithExternalHttpEndpoints();

    EndpointReference endpoint = otelCollector.GetEndpoint("http");
    apiService.WithEnvironment(
        name: "OTEL_EXPORTER_OTLP_ENDPOINT",
        endpointReference: endpoint);

    frontend.WithEnvironment(
        name: "OTEL_EXPORTER_OTLP_ENDPOINT",
        endpointReference: endpoint);

    builder
        .AddContainer(
            name: "jaeger",
            image: "jaegertracing/all-in-one")
        .WithEndpoint(
            port: 16686,
            targetPort: 16686,
            isExternal: true); // UI
}

builder
    .Build()
    .Run();
