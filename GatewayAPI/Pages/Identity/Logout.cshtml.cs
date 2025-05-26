using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GatewayAPI.Pages.Identity
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetLogout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToPage("/identity/login");
        }
    }
}
