using CSharpFunctionalExtensions;

namespace CourseService.Models.Interactivities
{
    public class TaskAnswer : Entity<Guid>
    {
        public Guid TaskId { get; set; }
        public Guid InteractiveResponceId{ get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public string SelectedOptions{ get; set; } = string.Empty;
    }
}
