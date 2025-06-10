using IdentityService.Data;
using IdentityService.Repositories;
using IdentityService.Services;
using Serilog;

namespace IdentityService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddGrpc();
        builder.Services.AddDbContext<IdentityDbContext>();
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()                     
            .WriteTo.Console()                                
        .CreateLogger();

        builder.Host.UseSerilog();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapGrpcService<Services.IdentityService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Run();
    }
}