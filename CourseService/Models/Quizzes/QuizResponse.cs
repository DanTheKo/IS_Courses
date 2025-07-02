


using CSharpFunctionalExtensions;

namespace CourseService.Models.Quizzes
{
    public class QuizResponse : Entity<Guid>
    {

        public Guid QuizId { get; set; }
        public Guid IdentityId { get; set; }
        public ICollection<QuestionAnswer> QuestionAnswers { get; set; } = new List<QuestionAnswer>();
        public QuizResponse(string id, string quizId, string identityId)
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
            IdentityId = Guid.Parse(identityId);
        }
        public QuizResponse()
        {

        }
    }
}
