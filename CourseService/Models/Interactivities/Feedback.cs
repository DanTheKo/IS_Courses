using CSharpFunctionalExtensions;

namespace CourseService.Models.Interactivities
{
    public class Feedback : Entity<Guid>
    {
        public Guid TaskAnswerId { get; set; }
        public Guid ExaminerId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}
