
using CSharpFunctionalExtensions;

namespace CourseService.Models.Quizzes
{
    public class QuestionAnswer : Entity<Guid>
    {
        public Question Question { get; set; }
        public Guid QuestionId { get; set; }
        public Guid QuizResponceId{ get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public string SelectedOptions{ get; set; } = string.Empty;
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

        public QuestionAnswer(string id, string questionId, string quizResponceId, string answerText, string selectedOptions)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Id = Guid.NewGuid();
            }
            else
            {
                Id = Guid.Parse(id);
            }
            QuestionId = Guid.Parse(questionId);
            QuizResponceId = Guid.Parse(quizResponceId);
            AnswerText = answerText;
            SelectedOptions = selectedOptions;
        }
        public QuestionAnswer()
        {
            
        }
    }
}
