using CourseService.Services;
using System;
using CourseService.Data;
using Microsoft.EntityFrameworkCore;
using CourseService.RabbitMq;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CourseService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddGrpc();
        builder.Services.AddDbContext<CourseDbContext>();
        builder.Services.AddScoped<Services.CourseService>();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
/*            .WriteTo.File("logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)*/
        .CreateLogger();
        builder.Host.UseSerilog();

/*        builder.Services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("CourseService"))
                    .SetSampler(new AlwaysOnSampler())
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddJaegerExporter(opt =>
                    {
                        opt.Endpoint = new Uri("http://localhost:14268/api/traces");
                        opt.Protocol = OpenTelemetry.Exporter.JaegerExportProtocol.HttpBinaryThrift;
                    });
            });*/


/*        builder.Services.AddHostedService<RabbitMqConsumerService>(provider =>
        {
            return new RabbitMqConsumerService(provider.GetRequiredService<IServiceScopeFactory>(),queue: "CoursesQueue");
        });*/

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapGrpcService<Services.CourseService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Run();
    }
}