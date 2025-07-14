using GatewayAPI.Grpc;
using GatewayAPI.Models.DTO;
using GatewayAPI.PageFilters;
using GatewayAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;



//Сделать контроллеры, написать сервисы интерактивов и профиля пользователя

namespace GatewayAPI.Pages.Courses
{
    [Authorize]
    [RedirectByAccess("id", "/index")]
    [BindProperties]
    public class EditorModel : PageModel
    {
        private readonly ILogger<EditorModel> _logger;
        public CourseServiceClient _courseClient;
        public QuizServiceClient _quizClient;
        public ProfileServiceClient _profileClient;

        public EditorModel(ILogger<EditorModel> logger, CourseServiceClient courseClient, QuizServiceClient quizClient, ProfileServiceClient profileClient)
        {
            _logger = logger;
            _courseClient = courseClient;
            _quizClient = quizClient;
            _profileClient = profileClient;
        }

        public Course CurrentCourse { get; set; }
        public List<CourseItem> CourseItems { get; set; } = new List<CourseItem>();
        public List<Content> Contents { get; set; } = new List<Content>();
        public CourseItem CurrentCourseItem { get; set; }
        public Quiz Quiz { get; set; }
        public List<Question> Questions { get; set; } = new();

        public CourseItemDto? NewCourseItem { get; set; } = new CourseItemDto();

        public async Task<IActionResult> OnGet(string id, string? idItem = "")
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
                    Console.WriteLine(CurrentCourseItem.QuizzesIds);
                    if (CurrentCourseItem.QuizzesIds != null && CurrentCourseItem.QuizzesIds.Count> 0)
                    {
                        Quiz = await _quizClient.GetQuizAsync(CurrentCourseItem.QuizzesIds[0]);
                        for (int i = 0; i < Quiz.QuestionsIds.Count; i++)
                        {
                            Questions.Add(await _quizClient.GetQuestionAsync(Quiz.QuestionsIds[i]));
                        }
                    }


                    Contents = new List<Content>();
                    for (int i = 0; i < CurrentCourseItem.ContentsIds.Count; i++)
                    {
                        Contents.Add(await _courseClient.GetContentAsync(CurrentCourseItem.ContentsIds[i]));
                    }
                    Contents = Contents.OrderBy(x => x.Order).ToList();
                    if (Contents.Count == 0)
                    {
                        Contents.Add(await _courseClient.CreateContentAsync(CurrentCourseItem.Id, "Base", "Текст..."));
                    }
                    var f = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToPage("/Error");
                throw;
            }
            return Page();
        }


        public async Task<IActionResult> OnPostSaveDataAsync([FromBody] CourseItem_ContentDto saveData)
        {
            try
            {
                if (string.IsNullOrEmpty(saveData.Content.Id))
                {
                    var list = new List<ContentDto>
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
            catch (Exception)
            {
                return new BadRequestResult();
                throw;
            }

        }



        public async Task<IActionResult> OnPostCreateCourseItemAsync(string id, int order, string newCourseItemTitle, string newCourseItemType)
        {
            try
            {
                CourseItem newItem = await _courseClient.CreateCourseItemAsync(id,
                    string.Empty,
                    newCourseItemType,
                    newCourseItemTitle,
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
        public async Task<IActionResult> OnPostDeleteCourseItemAsync([FromHeader] string itemId)
        {
            try
            {
                await _courseClient.DeleteCourseItemAsync(itemId);
                return new JsonResult(new { redirect = $"/courses/editor/{CurrentCourse.Id}" });
            }
            catch (Exception)
            {

                return new BadRequestResult();
                throw;
            }

        }

        public async Task<IActionResult> OnPostCreateQuizAsync([FromBody]Quiz_QuestionsDto? dto)
        {
            if (dto.Quiz == null || dto.Questions == null) return new BadRequestResult();
            try
            {

                bool quizExists = await _quizClient.GetQuizAsync(dto.Quiz.Id) != null;
                if (quizExists) 
                {
                    var quiz = await _quizClient.UpdateQuizAsync(dto.Quiz);
                    for (int i = 0; i < dto.Questions.Count; i++)
                    {
                        bool questionExists = await _quizClient.GetQuestionAsync(dto.Questions[i].Id) != null;
                        if (questionExists)
                        {
                            await _quizClient.UpdateQuestionAsync(dto.Questions[i]);
                        }
                        else
                        {
                            dto.Questions[i].QuizId = quiz.Id;
                            await _quizClient.CreateQuestionAsync(dto.Questions[i]);
                        }
                    }
                }
                else
                {
                    var quiz = await _quizClient.CreateQuizAsync(dto.Quiz);
                    foreach (var question in dto.Questions)
                    {
                        question.QuizId = quiz.Id;
                    }
                    await _quizClient.CreateQuestionsAsync(dto.Questions);

                }

                return new JsonResult(new { redirect = $"/courses/editor/{CurrentCourse.Id}" });

            }
            catch (Exception)
            {

                return new BadRequestResult();
                throw;
            }

        }

        public async Task<IActionResult> OnPostDeleteQuizAsync([FromHeader] string quizId)
        {
            try
            {
                await _quizClient.DeleteQuizAsync(quizId);
                return new JsonResult(new { redirect = $"/courses/editor/{CurrentCourse.Id}" });
            }
            catch (Exception)
            {

                return new BadRequestResult();
                throw;
            }

        }

        public async Task<IActionResult> OnPostDeleteQuestionAsync([FromHeader] string questionId)
        {
            try
            {
                await _quizClient.DeleteQuestionAsync(questionId);
                return new JsonResult(new { redirect = $"/courses/editor/{CurrentCourse.Id}" });
            }
            catch (Exception)
            {

                return new BadRequestResult();
                throw;
            }

        }


        public async Task<IActionResult> OnPostCreateContentsAsync([FromBody]List<Content> contents)
        {
            await _courseClient.CreateContentsAsync(contents);
            return new OkResult();
        }
    }
}
