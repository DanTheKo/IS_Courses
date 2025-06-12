using GatewayAPI.Grpc;
using GatewayAPI.PageFilters;
using GatewayAPI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GatewayAPI.Pages.Identity
{
    [RedirectAuthenticatedUsers]
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        public AuthorizationServiceClient _authClient;

        public LoginModel(ILogger<LoginModel> logger, AuthorizationServiceClient authClient)
        {
            _logger = logger;
            _authClient = authClient;
        }


        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        [BindProperty]
        public AuthenticationResponse Identity { get; set; }
        [BindProperty]
        public UserInfoResponse UserInfo { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Имя пользователя или Email обязятельны")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Пароль обязателен")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Запомнить меня")]
            public bool RememberMe { get; set; }

        }

        public async Task<IActionResult> OnPostLogin()
        {
            if (await IsValidUser(Input.Email, Input.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, UserInfo.Username),
                    new Claim(ClaimTypes.NameIdentifier, Identity.UserId),
                    new Claim(ClaimTypes.Role, Identity.Role)
                };

                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("Cookies", principal);
                return RedirectToPage($"/index");
            }

            ModelState.AddModelError("", "Неверный логин или пароль");
            return Page();
        }   

        private async Task<bool> IsValidUser(string login, string password)
        {
            Identity = await _authClient.AuthenticateAsync(login, password);
            if (Identity.Success)
            {
                UserInfo = await _authClient.GetUserInfoAsync(Identity.UserId);
            }
            return Identity.Success;
        }

    }
}