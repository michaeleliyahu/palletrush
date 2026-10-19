using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Telemetry.Api.Data;
using Telemetry.Api.Events;
using Telemetry.Api.Simulation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<TelemetryDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("Telemetry")));
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var options = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis")!);
    options.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(options);
});
builder.Services.AddScoped<CourseEventPublisher>();
builder.Services.AddScoped<RunSimulator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<TelemetryDbContext>().Database.MigrateAsync();
}

app.MapControllers();

app.Run();
