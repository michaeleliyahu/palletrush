using League.Api.Data;
using League.Api.Events;
using League.Api.Services;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<LeagueDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("League"))
    .UseAsyncSeeding(SeedData.SeedAsync));
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var options = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis")!);
    options.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(options);
});

builder.Services.AddScoped<HeatService>();
builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<LeaderboardService>();
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<TeamService>();

builder.Services.AddScoped<ICourseEventHandler<RunStarted>, RunStartedHandler>();
builder.Services.AddScoped<ICourseEventHandler<ConeHit>, ConeHitHandler>();
builder.Services.AddScoped<ICourseEventHandler<RunFinished>, RunFinishedHandler>();
builder.Services.AddHostedService<CourseEventsConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<LeagueDbContext>().Database.MigrateAsync();

    // Mimics the latency of the production search service (see ARCHITECTURE.md).
    app.Use(async (context, next) =>
    {
        if (HttpMethods.IsGet(context.Request.Method) && context.Request.Path == "/api/drivers")
            await Task.Delay(Random.Shared.Next(100, 1501));

        await next();
    });
}

app.MapControllers();

app.Run();
