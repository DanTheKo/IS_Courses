using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using System.Security.Claims;
namespace BlazorGatewayAPI.Services
{
    public class AuthService
    {
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            AuthenticationStateProvider authStateProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _authStateProvider = authStateProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LoginAsync(string email, string role = "User")
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Role, role)
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);


            // Аутентификация через HttpContext
            await _httpContextAccessor.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            // Уведомление Blazor о изменении состояния аутентификации
            NotifyStateChangedAsync();
        }

        public async Task LogoutAsync()
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            NotifyStateChangedAsync();
        }
        private async Task NotifyStateChangedAsync()
        {
            // Создаём новый AuthenticationState
            var authState = await _authStateProvider.GetAuthenticationStateAsync();

            // Если _authProvider — ServerAuthenticationStateProvider, он сам обработает уведомление
            if (_authStateProvider is ServerAuthenticationStateProvider serverAuth)
            {
                // В Blazor Server состояние обновляется автоматически после SignIn/SignOut
                // Явный вызов Notify не требуется!
            }
        }
    }
}
