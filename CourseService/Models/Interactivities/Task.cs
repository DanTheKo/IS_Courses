using CSharpFunctionalExtensions;

namespace CourseService.Models.Interactivities
{
    public class Task : Entity<Guid>
    {
        public Interactive Interactive { get; set; }
        public Guid InteractiveId { get; set; }
        public string TaskType { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public string Options { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public int MaxScore { get; set; }
        public int Order { get; set; }
    }
}
