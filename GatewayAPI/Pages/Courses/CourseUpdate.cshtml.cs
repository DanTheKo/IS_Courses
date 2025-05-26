using GatewayAPI.Grpc;
using GatewayAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace GatewayAPI.Pages.Courses
{
    [Authorize]
    public class CourseUpdateModel : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        private readonly ILogger<CourseCreateModel> _logger;
        public CourseServiceClient _courseClient;

        [BindProperty(SupportsGet = true)]
        public Course Course { get; set; }

        public CourseUpdateModel(ILogger<CourseCreateModel> logger, CourseServiceClient courseClient)
        {
            _logger = logger;
            _courseClient = courseClient;
        }
        public async Task<IActionResult> OnGet(string id)
        {
            Id = id;
            if(!string.IsNullOrEmpty(id)) Course = await _courseClient.GetCourseAsync(Id);
            if (Course == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _courseClient.UpdateCourseAsync(Id, Course.Title, Course.Description);

            return RedirectToPage("/Index");
        }
    }
}
