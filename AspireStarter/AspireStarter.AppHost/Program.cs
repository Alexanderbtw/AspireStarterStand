using MailDev.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var parameter = builder.AddParameter("parameterName", secret: false);
var secretParameter = builder.AddParameter("secretParameterName", secret: true);
var mssql = builder.AddConnectionString("mssql");

var cache = builder.AddRedis("cache");
var mailDev = builder.AddMailDev("maildev");
var apiService = builder.AddProject<Projects.AspireStarter_ApiService>("apiservice")
    .WithEnvironment("MY_PARAMETER", parameter)
    .WithEnvironment("MY_SECRET_PARAMETER", secretParameter)
    .WithReference(mssql);

var frontend = builder.AddProject<Projects.AspireStarter_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService)
    .WithReference(mailDev);

var reactFrontend = builder
    .AddNpmApp("reactfrontend", "../../ReactFrontend/react-frontend",
        builder.ExecutionContext.IsRunMode ? "dev" : "start")
    .WithReference(apiService)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

if (builder.ExecutionContext.IsPublishMode)
{
    // TODO: ELK Stack

    var otelCollector = builder
        .AddContainer("collector", "otel/opentelemetry-collector-contrib")
        .WithArgs("--config", "/etc/otel-collector.yaml")
        .WithBindMount("configs/otel-collector.yaml", "/etc/otel-collector.yaml", isReadOnly: true)
        .WithHttpEndpoint(55679, 55679, name: "zpages") // zpages
        .WithHttpEndpoint(4317, 4317) // gRPC
        .WithExternalHttpEndpoints();

    var endpoint = otelCollector.GetEndpoint("http");
    apiService.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", endpoint);
    frontend.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", endpoint);

    builder
        .AddContainer("jaeger", "jaegertracing/all-in-one")
        .WithEndpoint(16686, 16686, isExternal: true); // UI

}

builder.Build().Run();