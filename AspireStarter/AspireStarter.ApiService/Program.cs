WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddProblemDetails();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseCors();

string[] summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet(
    pattern: "/weatherforecast",
    handler: () =>
    {
        WeatherForecast[] forecast = Enumerable
            .Range(
                start: 1,
                count: 5)
            .Select(index =>
                new WeatherForecast(
                    Date: DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC: Random.Shared.Next(
                        minValue: -20,
                        maxValue: 55),
                    Summary: summaries[Random.Shared.Next(summaries.Length)]))
            .ToArray();

        return forecast;
    });

app.MapDefaultEndpoints();

app.Run();

internal sealed record WeatherForecast(
    DateOnly Date,
    int TemperatureC,
    string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
