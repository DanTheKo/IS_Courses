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

        public class InputModel
        {
            [Required(ErrorMessage = "Username or email required")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password required")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me")]
            public bool RememberMe { get; set; }

        }


        public async Task<IActionResult> OnPostLogin()
        {

            if (await IsValidUser(Input.Email, Input.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Input.Email),
                    new Claim(ClaimTypes.Role, "User")
                };

                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("Cookies", principal);
                return RedirectToPage($"/index");
            }

            ModelState.AddModelError("", "Incorrect login or password");
            return Page();
        }   

        private async Task<bool> IsValidUser(string login, string password)
        {
            var responce = await _authClient.AuthenticateAsync(login, password);
            return responce.Success;
        }

    }
}