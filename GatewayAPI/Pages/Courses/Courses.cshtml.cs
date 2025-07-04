using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GatewayAPI.Grpc;
using GatewayAPI.Services;

namespace GatewayAPI.Pages.Courses
{
    public class CoursesModel : PageModel
    {
        private readonly ILogger<CoursesModel> _logger;
        public CourseServiceClient _courseClient;

        public CoursesModel(ILogger<CoursesModel> logger, CourseServiceClient courseClient)
        {
            _logger = logger;
            _courseClient = courseClient;
        }
        [BindProperty]
        public int PageNumber { get; set; } = 0;


        [BindProperty]
        public List<Course> Courses { get; set; } = new List<Course>();
        public async Task<IActionResult> OnGet(int pageNumber)
        {
            PageNumber = pageNumber;
            try
            {
                var responce = await _courseClient.GetCoursesAsync(PageNumber, 30);
                var items = responce.Items.Reverse();
                Courses.AddRange(items);
            }
            catch (Exception)
            {

                throw;
            }
            return Page();
        }
    }
}
