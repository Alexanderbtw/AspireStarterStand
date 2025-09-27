using MailDev.Hosting;

using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> parameter = builder.AddParameter(
    name: "parameterName",
    secret: false);

IResourceBuilder<ParameterResource> secretParameter = builder.AddParameter(
    name: "secretParamName",
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

builder.AddDockerComposeEnvironment("compose");

DistributedApplication app = builder.Build();

app.Run();
