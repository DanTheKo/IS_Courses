using IdentityService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GatewayAPI.Pages.Identity
{
    public class ProfileModel : PageModel
    {
        [BindProperty]
        public Profile Profile { get; set; }

        public void OnGet()
        {
            // Загружаем профиль из базы данных
            // Это пример - замените на реальную логику
            Profile = new Profile(Guid.NewGuid())
            {
                FirstName = "Иван",
                LastName = "Иванов",
                MiddleName = "Иванович",
                Status = "Активный пользователь",
                ProfileImageUrl = "/images/avatar.jpg"
            };
        }

        public async Task<IActionResult> OnPostSaveAsync([FromBody] Profile profile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Некорректные данные профиля");
                }

                // Здесь сохраняем профиль в базу данных
                // Это пример - замените на реальную логику
                Profile = profile;

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
