using GatewayAPI.PageFilters;
using GatewayAPI.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GatewayAPI.Pages.Identity
{
    [RedirectAuthenticatedUsers]
    public class RegisterModel : PageModel
    {
        private readonly IdentityServiceClient _authClient;
        private readonly ILogger<RegisterModel> _logger;
        public RegisterModel(ILogger<RegisterModel> logger, IdentityServiceClient authClient)
        {
            _logger = logger;
            _authClient = authClient;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [Display(Name = "��� ������������")]
            public string Login { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "������ ������ ���� �� {2} �� {1} ��������.", MinimumLength = 1)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "������������� ������")]
            [Compare("Password", ErrorMessage = "������ �� ���������")]
            public string ConfirmPassword { get; set; }

            [Phone]
            [Display(Name = "�������")]
            public string? Phone { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var responce = await _authClient.RegisterAsync(Input.Login, Input.ConfirmPassword, Input.Email, Input.Phone);
            if(responce.Success != false)
            {
                return RedirectToPage("/identity/login");
            }
            else
            {
                ErrorMessage = responce.Errors.FirstOrDefault();
                return Page();
            }
        }
    }
}
