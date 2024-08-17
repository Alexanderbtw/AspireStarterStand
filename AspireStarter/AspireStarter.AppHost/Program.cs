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

builder.AddProject<Projects.AspireStarter_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService)
    .WithReference(mailDev);

builder.Build().Run();