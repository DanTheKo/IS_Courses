

using CSharpFunctionalExtensions;

namespace CourseService.Models.Quizzes
{
    public class Quiz : Entity<Guid>
    {
        public CourseItem CourseItem { get; set; }
        public Guid CourseItemId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<QuizResponse> QuizResponses { get; set; } = new List<QuizResponse>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();

        public Quiz(string id, string courseItemId, string title, string type, string description)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Id = Guid.NewGuid();
            }
            else
            {
                Id = Guid.Parse(id);
            }
            CourseItemId = Guid.Parse(courseItemId);
            Title = title;
            Type = type;
            Description = description;
        }

        public Quiz()
        {

        }
    }
}
