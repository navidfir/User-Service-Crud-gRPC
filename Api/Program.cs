using Api.Extensions;
using Api.Interceptors;
using Api.Services;
using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg
        .Enrich.FromLogContext()
        .Enrich.WithCorrelationId()
        .WriteTo.Console();
});

builder.Services.AddApiServices();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

// builder.Services.AddOpenTelemetry()
//     .WithTracing(t =>
//     {
//         t.AddAspNetCoreInstrumentation()
//          .AddEntityFrameworkCoreInstrumentation();
        //  .AddConsoleExporter();
    // });
    // .WithMetrics(m =>
    // {
    //     m.AddAspNetCoreInstrumentation()
    //      .AddRuntimeInstrumentation()
    //      .AddConsoleExporter();
    // });

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ExceptionInterceptor>();
    options.Interceptors.Add<LoggingInterceptor>();
});

// needs to be changed from static to dynamic
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5247, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});

var app = builder.Build();

app.MapGrpcService<UserGrpcService>();

app.MapHealthChecks("/health");

// Disable Nexr For Unit Tests
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate();
}

app.Run();

public partial class Program { }