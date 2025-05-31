using GatewayAPI.Grpc;
using GatewayAPI.PageFilters;
using GatewayAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GatewayAPI.Pages.Courses
{
    [Authorize]
    [RedirectByAccess("id", "/index")]
    public class ViewModel : PageModel
    {
        private readonly ILogger<ViewModel> _logger;
        public CourseServiceClient _courseClient;

        public ViewModel(ILogger<ViewModel> logger, CourseServiceClient courseClient)
        {
            _logger = logger;
            _courseClient = courseClient;
        }

        [BindProperty]
        public Course CurrentCourse { get; set; }
        [BindProperty]
        public CourseItem CurrentCourseItem { get; set; }
        [BindProperty]
        public List<Content> Contents { get; set; } = new List<Content>();
        [BindProperty]
        public List<Content> NewContents { get; set; } = new List<Content>();
        [BindProperty]
        public List<CourseItem> CourseItems { get; set; } = new List<CourseItem>();



        //Брокер сообщений RabbitMQ, логирование Garfana?, Jaeger, тесты Unit test 
        //Какой-то ElasticSearch
        public async Task<IActionResult> OnGet(string id, string idItem)
        {
            try
            {
                CurrentCourse = await _courseClient.GetCourseAsync(id);
                if (CurrentCourse == null)
                {
                    return NotFound();
                }
                if (CurrentCourse.CourseItemsIds.Count > 0)
                {
                    CourseItems = new List<CourseItem>();
                    for (int i = 0; i < CurrentCourse.CourseItemsIds.Count; i++)
                    {
                        CourseItems.Add(await _courseClient.GetCourseItemAsync(CurrentCourse.CourseItemsIds[i]));
                    }
                    CourseItems = CourseItems.OrderBy(x => x.Order).ToList();
                    if (!string.IsNullOrEmpty(idItem)) { CurrentCourseItem = await _courseClient.GetCourseItemAsync(idItem); }

                    if (CurrentCourseItem == null || !CourseItems.Contains(CurrentCourseItem)) { CurrentCourseItem = CourseItems.FirstOrDefault(); }
                    Console.WriteLine(CurrentCourseItem.ContentsIds);

                    Contents = new List<Content>();
                    for (int i = 0; i < CurrentCourseItem.ContentsIds.Count; i++)
                    {
                        Contents.Add(await _courseClient.GetContentAsync(CurrentCourseItem.ContentsIds[i]));
                    }
                    Contents = Contents.OrderBy(x => x.Order).ToList();
                }

            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
                throw;
            }
            return Page();
        }
    }
}

