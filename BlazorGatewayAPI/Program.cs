using BlazorGatewayAPI.Components;
using BlazorGatewayAPI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace BlazorGatewayAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.Name = "AuthCookie";
            options.LoginPath = "/login"; // Путь к странице входа
            options.LogoutPath = "/logout";
            options.AccessDeniedPath = "/error"; // Путь при отказе в доступе
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

        builder.Services.AddSingleton<AuthorizationServiceClient>(provider =>
        {
            var serviceUrl = "http://localhost:5125";
            return new AuthorizationServiceClient(serviceUrl);
        });
        builder.Services.AddSingleton<CourseServiceClient>(provider =>
        {
            var serviceUrl = "http://localhost:5057";
            return new CourseServiceClient(serviceUrl);
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
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
