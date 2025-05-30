using GatewayAPI.Grpc;
using GatewayAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GatewayAPI.Pages.Courses
{
    public class PreviewModel : PageModel
    {
        private readonly ILogger<PreviewModel> _logger;
        public CourseServiceClient _courseClient;

        public PreviewModel(ILogger<PreviewModel> logger, CourseServiceClient courseClient)
        {
            _logger = logger;
            _courseClient = courseClient;
        }

        [BindProperty]
        public Course Course { get; set; }
        public async Task<IActionResult> OnGet(string id)
        {
            try
            {
                Course = await _courseClient.GetCourseAsync(id);
            }
            catch (Exception)
            {

                throw;
            }
            return Page();
        }
    }
}
