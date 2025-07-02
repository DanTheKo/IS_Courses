
using CourseService.Models.Quizzes;
using CSharpFunctionalExtensions;

namespace CourseService.Models
{
    public class CourseItem : Entity<Guid>
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid? ParentId { get; set; }
        public CourseItem? Parent { get; set; }

        public string Title { get; set; }
        public string Type { get; set; }
        public int Order { get; set; }

        public ICollection<CourseItem> Children { get; set; } = new List<CourseItem>();
        public ICollection<Content> Contents { get; set; } = new List<Content>();
        public ICollection<Quiz> Quizes { get; set; } = new List<Quiz>();


        public CourseItem(Guid id, CourseItem? parent, string title, string type, List<CourseItem> children, List<Content> contents, int order, Course course)
        {
            Id = id;
            ParentId = parent != null ? parent.Id : null;
            Parent = parent;
            Title = title;
            Type = type;
            Children = children;
            Contents = contents;
            Order = order;
            Course = course;
        }

        public CourseItem(Guid id)
        {
            Id = id;
        }


    }


}
