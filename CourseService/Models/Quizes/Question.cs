
using CSharpFunctionalExtensions;

namespace CourseService.Models.Quizes
{
    public class Question : Entity<Guid>
    {

        public Guid QuizId { get; set; }
        public string QuestionType { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public string Options { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public int MaxScore { get; set; }
        public int Order { get; set; }


        public Question(string id, string quizId, string questionType, string questionText, string options, string correctAnswer, int maxScore, int order)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Id = Guid.NewGuid();
            }
            else
            {
                Id = Guid.Parse(id);
            }
            QuizId = Guid.Parse(quizId);
            QuestionType = questionType;
            QuestionText = questionText;
            Options = options;
            CorrectAnswer = correctAnswer;
            MaxScore = maxScore;
            Order = order;
        }

        public Question()
        {

        }

    }
}
