using GatewayAPI.Grpc;
using GatewayAPI.Models.DTO;
using GatewayAPI.PageFilters;
using GatewayAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace GatewayAPI.Pages.Courses
{
    [Authorize]
    [RedirectByAccess("id", "/index")]
    [BindProperties]
    public class ViewModel : PageModel
    {
        private readonly ILogger<ViewModel> _logger;
        public CourseServiceClient _courseClient;
        public QuizServiceClient _quizClient;

        public ViewModel(ILogger<ViewModel> logger, CourseServiceClient courseClient, QuizServiceClient quizClient)
        {
            _logger = logger;
            _courseClient = courseClient;
            _quizClient = quizClient;
        }

        public Course CurrentCourse { get; set; }
        public CourseItem CurrentCourseItem { get; set; }
        public List<Content> Contents { get; set; } = new List<Content>();
        public List<CourseItem> CourseItems { get; set; } = new List<CourseItem>();
        public Quiz Quiz { get; set; }
        public List<Question> Questions { get; set; } = new();

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

                    if (CurrentCourseItem == null || !CourseItems.Contains(CurrentCourseItem)) { CurrentCourseItem = CourseItems.FirstOrDefault(); }
                    if (CurrentCourseItem.QuizzesIds != null && CurrentCourseItem.QuizzesIds.Count > 0)
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
                }

            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
                throw;
            }
            return Page();
        }


        public async Task<IActionResult> OnPostSubmitQuizResponseAsync([FromBody] QuizResponse_QuestionAnswersDto dto)
        {
            try
            {
                if (dto == null || dto.QuizResponse == null || dto.QuestionAnswers == null) return new BadRequestResult();

                dto.QuizResponse.IdentityId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


                //QuizResponse quizResponse = await _quizClient.CreateQuizResponseAsync(dto.QuizResponse);
                foreach (var questionAnswer in dto.QuestionAnswers)
                {
                    //questionAnswer.QuizResponseId = quizResponse.Id;
                    //await _quizClient.CreateQuestionAnswerAsync(questionAnswer);
                }

                return new JsonResult( GetQuizResult(dto.Questions, dto.QuestionAnswers));
            }
            catch (Exception)
            {

                return new BadRequestResult();
                throw;
            }

        }
        public QuizResultDto GetQuizResult(List<Question> questions, List<QuestionAnswer> questionAnswers)
        {
            questions.OrderBy(q => q.Order);
            var quizResultDto = new QuizResultDto
            {
                Questions = questions,
                QuestionAnswers = questionAnswers,
                Results = new List<QuestionResultDto>(),
                TotalScore = 0,
                MaxPossibleScore = questions.Sum(q => q.MaxScore)
            };


            foreach (var question in questions)
            {
                var userAnswer = questionAnswers.FirstOrDefault(qa => qa.QuestionId == question.Id);
                var isCorrect = false || question.CorrectAnswer == "Any";
                var userScore = 0;

                if (userAnswer != null && !isCorrect)
                {
                    
                    switch (question.QuestionType)
                    {
                        case "SingleChoice":
                            isCorrect = userAnswer.AnswerText.Equals(question.CorrectAnswer, StringComparison.OrdinalIgnoreCase);
                            break;

                        case "MultipleChoice":
                            var correctAnswers = question.CorrectAnswer.Split(' ');
                            var userAnswers = userAnswer.AnswerText.Split(' ');
                            isCorrect = correctAnswers.All(ca => userAnswers.Contains(ca)) &&
                                        userAnswers.All(ua => correctAnswers.Contains(ua));
                            break;

                        case "TextAnswer":
                            isCorrect = string.Equals(
                                userAnswer.AnswerText.Trim(),
                                question.CorrectAnswer.Trim(),
                                StringComparison.OrdinalIgnoreCase);
                            break;
                    }

                }
                userScore = isCorrect ? question.MaxScore : 0;
                quizResultDto.TotalScore += userScore;

                quizResultDto.Results.Add(new QuestionResultDto
                {
                    QuestionId = question.Id,
                    QuestionText = question.QuestionText,
                    CorrectAnswer = question.CorrectAnswer,
                    UserAnswer = userAnswer?.AnswerText ?? "NoAnswer",
                    IsCorrect = isCorrect,
                    Score = userScore,
                    MaxScore = question.MaxScore
                });
            }
            return quizResultDto;
        }
    }
}

