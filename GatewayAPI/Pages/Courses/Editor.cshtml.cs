using GatewayAPI.Grpc;
using GatewayAPI.PageFilters;
using GatewayAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using static GatewayAPI.Pages.Courses.CourseCreateModel;


namespace GatewayAPI.Pages.Courses
{
    //[Authorize]
    //[RedirectByAccess("id", "/index")]
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
        public DTOCourse _CurrentCourse { get; set; }
        [BindProperty]
        public CourseItem CurrentCourseItem { get; set; }
        [BindProperty]
        public DTOCourseItem NewCourseItem { get; set; } = new DTOCourseItem();
        [BindProperty]
        public List<Content> Contents { get; set; } = new List<Content>();
        [BindProperty]
        public List<CourseItem> CourseItems { get; set; } = new List<CourseItem>();

        //Брокер сообщений RabbitMQ, логирование Serilog, Jaeger, тесты Unit test 
        //Какой-то ElasticSearch
        public async Task<IActionResult> OnGet(string id, string? idItem)
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

                    if (CurrentCourseItem == null || CurrentCourseItem.CourseId != CurrentCourse.Id) { CurrentCourseItem = CourseItems.First(); }

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
            try
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
            catch (Exception ex)
            {
                return new BadRequestResult();
                throw;
            }

        }



        public async Task<IActionResult> OnPostCreateCourseItemAsync(string id, int order)
        {
            try
            {
                CourseItem newItem = await _courseClient.CreateCourseItemAsync(id,
                    string.Empty,
                    string.IsNullOrWhiteSpace(NewCourseItem.Type) ? "Type" : NewCourseItem.Type,
                    string.IsNullOrWhiteSpace(NewCourseItem.Title) ? "Title" : NewCourseItem.Title,
                    order);

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
            catch (Exception)   
            {

                return new BadRequestResult();
                throw;
            }

        }

        public async Task<IActionResult> OnPostUpdateCourseAsync(string courseId, string currentCourseTitle, string currentCourseDescription)
        {
            try
            {
                Course newItem = await _courseClient.UpdateCourseAsync(courseId, currentCourseTitle, currentCourseDescription);
                return Redirect($"/courses/editor/{courseId}");
            }
            catch (Exception)
            {

                return new BadRequestResult();
                throw;
            }

        }
        public class Entity
        {
            public string Id { get; set; }
        }
        public async Task<IActionResult> OnPostDeleteCourseItemAsync([FromBody] Entity entity)
        {
            await _courseClient.DeleteCourseItemAsync(entity.Id);
            return Redirect($"/courses/editor/{CurrentCourse.Id}");
        }

        public async Task<IActionResult> OnPostCreateContentsAsync([FromBody]List<Content> contents)
        {
            await _courseClient.CreateContentsAsync(contents);
            return new OkResult();
        }
    }

    public class DTOCourseItem
    {
        public string Title { get; set; }
        public string Type { get; set; }
    }
}
