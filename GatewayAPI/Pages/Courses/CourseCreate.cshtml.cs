using GatewayAPI.Services;
using GatewayAPI.Grpc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GatewayAPI.Models.DTO;

namespace GatewayAPI.Pages.Courses
{
    [Authorize]
    public class CourseCreateModel : PageModel
    {
        
        private readonly ILogger<CourseCreateModel> _logger;
        public CourseServiceClient _courseClient;
        public AccessServiceClient _accessClient;

        [BindProperty]
        public CourseDto Course { get; set; } = new();

        public CourseCreateModel(ILogger<CourseCreateModel> logger, CourseServiceClient courseClient, AccessServiceClient accessClient)
        {
            _logger = logger;
            _courseClient = courseClient;
            _accessClient = accessClient;
        }
        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string title, string description)
        {

            var responce = await _courseClient.CreateCourseAsync(title, description);

            string? identityId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? resourseId = responce.Id;

            await _accessClient.CreateAccessAsync(identityId, resourseId, "Owner");

            return Redirect($"/Courses");
        }
    }
}
