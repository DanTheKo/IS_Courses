using GatewayAPI.Services;
using GatewayAPI.Grpc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace GatewayAPI.Pages.Courses
{
    [Authorize]
    public class CourseCreateModel : PageModel
    {
        
        private readonly ILogger<CourseCreateModel> _logger;
        public CourseServiceClient _courseClient;

        [BindProperty]
        public Course Course { get; set; } = new();

        public CourseCreateModel(ILogger<CourseCreateModel> logger, CourseServiceClient courseClient)
        {
            _logger = logger;
            _courseClient = courseClient;
        }
        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var responce = await _courseClient.CreateCourseAsync(Course.Title, Course.Description);
            
            return Redirect($"/Courses/CourseUpdate/{responce.Id}");
        }
    }
}
