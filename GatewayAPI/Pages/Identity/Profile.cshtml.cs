using GatewayAPI.Grpc;
using GatewayAPI.Models.DTO;
using GatewayAPI.Pages.Courses;
using GatewayAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace GatewayAPI.Pages.Identity
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly ILogger<ProfileModel> _logger;
        public CourseServiceClient _courseClient;
        public QuizServiceClient _quizClient;
        public ProfileServiceClient _profileClient;

        public ProfileModel(ILogger<ProfileModel> logger, ProfileServiceClient profileClient)
        {
            _logger = logger;
            _profileClient = profileClient;
        }
        [BindProperty]
        public Profile Profile { get; set; }

        public async void OnGet()
        {
            Profile = await _profileClient.GetProfileByIdentityIdAsync(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if(Profile == null)
            {
                Profile profile = new Profile();
                profile.IdentityId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                profile.Firstname = "";
                profile.Lastname = "";
                profile.Middlename = "";
                profile.Status = "";
                profile.ProfileImageUrl = "";

                Profile = await _profileClient.CreateProfileAsync(profile);
            }

            Console.WriteLine(new JsonResult(Profile).Value);
        }

        public async Task<IActionResult> OnGetProfile()
        {
            Profile = await _profileClient.GetProfileByIdentityIdAsync(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (Profile == null)
            {
                Profile profile = new Profile();
                profile.IdentityId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                profile.Firstname = "";
                profile.Lastname = "";
                profile.Middlename = "";
                profile.Status = "";
                profile.ProfileImageUrl = "";

                Profile = await _profileClient.CreateProfileAsync(profile);
            }

            Console.WriteLine(new JsonResult(Profile).Value);
            return new JsonResult(Profile);
        }


        public async Task<IActionResult> OnPostSaveProfileAsync([FromBody] ProfileDto dto)
        {
            try
            {
                var profile = new Profile
                {
                    Id = dto.id,
                    IdentityId = dto.identityId,
                    Firstname = dto.firstname,
                    Lastname = dto.lastname,
                    Middlename = dto.middlename,
                    Status = dto.status,
                    ProfileImageUrl = dto.profileImageUrl
                };

                await _profileClient.UpdateProfileAsync(profile);

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
