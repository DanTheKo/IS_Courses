using CSharpFunctionalExtensions;

namespace CourseService.Models.Interactivities
{
    public class Interactive : Entity<Guid>
    {
        public CourseItem CourseItem { get; set; }
        public Guid CourseItemId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
