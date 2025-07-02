

using CSharpFunctionalExtensions;

namespace CourseService.Models.Quizzes
{
    public class Feedback : Entity<Guid>
    {

        public QuestionAnswer QuestionAnswer { get; set; }
        public Guid QuestionAnswerId { get; set; }
        public Guid ExaminerId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }

        public Feedback(string id,string questionAnswerId, string examinerId, string comment, int rating)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Id = Guid.NewGuid();
            }
            else
            {
                Id = Guid.Parse(id);
            }
            QuestionAnswerId = Guid.Parse(questionAnswerId);
            ExaminerId = Guid.Parse(examinerId);
            Comment = comment;
            Rating = rating;
        }

        public Feedback()
        {

        }
    }
}
