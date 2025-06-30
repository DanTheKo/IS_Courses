using Microsoft.AspNetCore.Authentication.Cookies;
using Grpc.Core;
using GatewayAPI.Services;
using GatewayAPI.RabbitMq;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using AutoMapper;
using GatewayAPI.Grpc;

namespace GatewayAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "AuthCookie";
                options.LoginPath = "/Identity/Login"; // Путь к странице входа
                options.AccessDeniedPath = "/Error"; // Путь при отказе в доступе
            });
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
/*                .WriteTo.File("logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7)*/
            .CreateLogger();

            builder.Host.UseSerilog();


/*            builder.Services.AddOpenTelemetry()
                .WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault()
                            .AddService("Gateway"))
                        .SetSampler(new AlwaysOnSampler())
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddJaegerExporter(opt =>
                        {
                            opt.Endpoint = new Uri("http://localhost:14268/api/traces");
                            opt.Protocol = OpenTelemetry.Exporter.JaegerExportProtocol.HttpBinaryThrift;
                        });
                });*/


/*            builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>(provider =>
            {
                return new RabbitMqService(queue: "CoursesQueue");
            });*/
            {
                builder.Services.AddSingleton<IdentityServiceClient>(provider =>
                {
                    var serviceUrl = "http://localhost:5125";
                    return new IdentityServiceClient(serviceUrl);
                });
                builder.Services.AddSingleton<CourseServiceClient>(provider =>
                {
                    var serviceUrl = "http://localhost:5057";
                    return new CourseServiceClient(serviceUrl/*, provider.GetRequiredService<IRabbitMqService>()*/);
                });
                builder.Services.AddSingleton<QuizServiceClient>(provider =>
                {
                    var serviceUrl = "http://localhost:5057";
                    return new QuizServiceClient(serviceUrl/*, provider.GetRequiredService<IRabbitMqService>()*/);
                });
                builder.Services.AddSingleton<AccessServiceClient>(provider =>
                {
                    var serviceUrl = "http://localhost:5122";
                    return new AccessServiceClient(serviceUrl);
                });


                var app = builder.Build();


                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapRazorPages();


                app.Run();
            }
        }

    }
}
