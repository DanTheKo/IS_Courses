using CourseService.Services;
using static CSharpFunctionalExtensions.Result;
using System;
using CourseService.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddGrpc();
        builder.Services.AddDbContext<CourseDbContext>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapGrpcService<Services.CourseService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Run();
    }
}