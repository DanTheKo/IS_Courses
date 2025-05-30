using Microsoft.AspNetCore.Authentication.Cookies;
using Grpc.Core;
using GatewayAPI.Services;

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
