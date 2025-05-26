using CSharpFunctionalExtensions;

namespace CourseService.Models.NotUsed
{
    public class Module : Entity<Guid>
    {
        public string Title { get; set; }
        public int Order { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public List<Page> Pages { get; set; } = new();

        public Module(string title, int order, int courseId, Course course, List<Page> pages)
        {
            Id = Guid.NewGuid();
            Title = title;
            Order = order;
            CourseId = courseId;
            Course = course;
            Pages = pages;
        }
    }
}
