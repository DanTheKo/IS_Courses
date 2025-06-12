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
        public DTOCourse Course { get; set; } = new();

        public CourseCreateModel(ILogger<CourseCreateModel> logger, CourseServiceClient courseClient)
        {
            _logger = logger;
            _courseClient = courseClient;
        }
        public IActionResult OnGet()
        {
            return Page();
        }


        public class DTOCourse
        {
            public string Title { get; set; }
            public string Description { get; set; }

        }

        public async Task<IActionResult> OnPostAsync(string title, string description)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var responce = await _courseClient.CreateCourseAsync(title, description);
            
            return Redirect($"/Courses");
        }
    }
}
