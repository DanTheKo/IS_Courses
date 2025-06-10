using GatewayAPI.Grpc;
using GatewayAPI.PageFilters;
using GatewayAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;


namespace GatewayAPI.Pages.Courses
{
    [Authorize]
    [RedirectByAccess("id", "/index")]
    public class EditorModel : PageModel
    {
        private readonly ILogger<EditorModel> _logger;
        public CourseServiceClient _courseClient;

        public EditorModel(ILogger<EditorModel> logger, CourseServiceClient courseClient)
        {
            _logger = logger;
            _courseClient = courseClient;
        }

        [BindProperty]
        public Course CurrentCourse { get; set; }
        [BindProperty]
        public CourseItem CurrentCourseItem { get; set; }
        [BindProperty]
        public CourseItem NewCourseItem { get; set; } = new CourseItem();
        [BindProperty]
        public List<Content> Contents { get; set; } = new List<Content>();
        [BindProperty]
        public List<CourseItem> CourseItems { get; set; } = new List<CourseItem>();

        //Брокер сообщений RabbitMQ, логирование Serilog, Jaeger, тесты Unit test 
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

                    Contents = new List<Content>();
                    for (int i = 0; i < CurrentCourseItem.ContentsIds.Count; i++)
                    {
                        Contents.Add(await _courseClient.GetContentAsync(CurrentCourseItem.ContentsIds[i]));
                    }
                    Contents = Contents.OrderBy(x => x.Order).ToList();
                    if(Contents.Count == 0)
                    {
                        Contents.Add(await _courseClient.CreateContentAsync(CurrentCourseItem.Id, "Base", "Текст..."));
                    }
                }

            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
                throw;
            }
            return Page();
        }

        public class SaveDataRequest
        {
            public Content Content { get; set; }
            public CourseItem CourseItem { get; set; }
        }

        public async Task<IActionResult> OnPostSaveDataAsync([FromBody] SaveDataRequest saveData)
        {
            if (string.IsNullOrEmpty(saveData.Content.Id))
            {
                var list = new List<Content>
                {
                    saveData.Content
                };
                await _courseClient.CreateContentAsync(saveData.Content.Id, saveData.Content.Type, saveData.Content.Data);

            }
            else
            {
                await _courseClient.UpdateCourseItemAsync(saveData.CourseItem.Id, saveData.CourseItem.Title, saveData.CourseItem.Type);
                await _courseClient.UpdateContentAsync(saveData.Content.Id, saveData.Content.Type, saveData.Content.Data);
            }
            return new OkResult();
        }

        public async Task<IActionResult> OnPostCreateCourseItemAsync(string id, int order)
        {
            CourseItem newItem = await _courseClient.CreateCourseItemAsync(id, string.Empty, string.Empty + NewCourseItem.Type, string.Empty + NewCourseItem.Title, order);
           
            List<Content> contents = new List<Content>();
            Content content = new Content();
            content.Order = 0;
            content.CourseItemId = newItem.Id;
            content.Data = "Текст...";
            content.Type = "Base";
            contents.Add(content);
            await _courseClient.CreateContentsAsync(contents);

            return Redirect($"/courses/editor/{id}/{newItem.Id}");
        }

        public class Entity
        {
            public string Id { get; set; }
        }
        public async Task<IActionResult> OnPostDeleteCourseItemAsync([FromBody] Entity entity)
        {
            await _courseClient.DeleteCourseItemAsync(entity.Id);

            return new OkResult();
        }

        public async Task<IActionResult> OnPostCreateContentsAsync([FromBody]List<Content> contents)
        {
            await _courseClient.CreateContentsAsync(contents);
            return new OkResult();
        }
    }
}
