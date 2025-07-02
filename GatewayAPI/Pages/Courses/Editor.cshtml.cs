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

        public EditorModel(ILogger<EditorModel> logger, CourseServiceClient courseClient, QuizServiceClient quizClient)
        {
            _logger = logger;
            _courseClient = courseClient;
            _quizClient = quizClient;
        }

        public Course CurrentCourse { get; set; }
        public List<CourseItem> CourseItems { get; set; } = new List<CourseItem>();
        public List<Content> Contents { get; set; } = new List<Content>();
        public CourseItem CurrentCourseItem { get; set; }
       
        
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
                    Quiz quiz = await _quizClient.CreateQuizAsync(new Quiz()
                    {
                        Id = "",
                        Title = "test",
                        Type = "test",
                        Description = "test",
                        CourseItemId = CurrentCourseItem.Id
                    });
                    Console.WriteLine(quiz.Id + "*&**&*&*&");
                    quiz = await _quizClient.GetQuizAsync(quiz.Id);
                    quiz = await _quizClient.UpdateQuizAsync(quiz);

                    QuizResponse quizResponse = await _quizClient.CreateQuizResponseAsync(new QuizResponse()
                    {
                        Id = "",
                        QuizId = quiz.Id,
                        IdentityId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    });

                    var test = await _quizClient.GetQuizResponseAsync(quizResponse.Id);
                    quizResponse = await _quizClient.UpdateQuizResponseAsync(quizResponse);

                    Question question = await _quizClient.CreateQuestionAsync(new Question()
                    {
                        Id = "",
                        QuizId = quiz.Id,
                        MaxScore = 5,
                        CorrectAnswer = "Test0",
                        Options = "Test0, Test1, Test2",
                        Order = 0
                    });
                    question = await _quizClient.GetQuestionAsync(question.Id);
                    question = await _quizClient.UpdateQuestionAsync(question);

                    QuestionAnswer questionAnswer = await _quizClient.CreateQuestionAnswerAsync(new QuestionAnswer()
                    {
                        Id = "",
                        SelectedOptions = "Test0, Test1, Test2",
                        QuestionId = question.Id,
                        QuizResponseId = quizResponse.Id


                    });
                    questionAnswer = await _quizClient.GetQuestionAnswerAsync(questionAnswer.Id);
                    questionAnswer = await _quizClient.UpdateQuestionAnswerAsync(questionAnswer);
                    Feedback feedback = await _quizClient.CreateFeedbackAsync(new Feedback()
                    {
                        Id = "",
                        ExaminerId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                        Comment = "test comment",
                        QuestionAnswerId = question.Id,
                        Rating = 5
                    });
                    feedback = await _quizClient.GetFeedbackAsync(feedback.Id);
                    feedback = await _quizClient.UpdateFeedbackAsync(feedback);

                    await _quizClient.DeleteFeedbackAsync(feedback.Id);
                    await _quizClient.DeleteQuizResponseAsync(quizResponse.Id);
                    await _quizClient.DeleteQuestionAnswerAsync(questionAnswer.Id);
                    await _quizClient.DeleteQuestionAsync(question.Id);
                    await _quizClient.DeleteQuizAsync(quiz.Id);

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

        public async Task<IActionResult> OnPostCreateContentsAsync([FromBody]List<Content> contents)
        {
            await _courseClient.CreateContentsAsync(contents);
            return new OkResult();
        }
    }
}
