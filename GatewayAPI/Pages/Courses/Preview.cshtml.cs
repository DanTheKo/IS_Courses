using GatewayAPI.Grpc;
using GatewayAPI.PageFilters;
using GatewayAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace GatewayAPI.Pages.Courses
{
    [Authorize]
    public class PreviewModel : PageModel
    {
        private readonly ILogger<PreviewModel> _logger;
        public CourseServiceClient _courseClient;
        public AccessServiceClient _accessClient;

        public PreviewModel(ILogger<PreviewModel> logger, CourseServiceClient courseClient, AccessServiceClient accessClient)
        {
            _logger = logger;
            _courseClient = courseClient;
            _accessClient = accessClient;
        }

        [BindProperty]
        public Course Course { get; set; }
        [BindProperty]
        public bool Access { get; set; }
        [BindProperty]
        public string AccessData { get; set; }
        public async Task<IActionResult> OnGet(string id)
        {
            try
            {
                Course = await _courseClient.GetCourseAsync(id);

                string? identityId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string? resourseId = Course.Id;
                AccessResponce responce = await _accessClient.GetAccessAsync(identityId, resourseId);
                Access = responce.HasAccess;
                AccessData = responce.AccessData;
            }
            catch (Exception)
            {

                throw;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGetAccessAsync()
        {
            try
            {
                string? identityId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string? resourseId = Course.Id;

                if (!Access)
                {
                    await _accessClient.CreateAccessAsync(identityId, resourseId, "Student");
                }
                
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteCourseAsync(string courseId)
        {
            await _courseClient.DeleteCourseAsync(courseId);
            return Redirect($"/courses");
        }

    }
}
