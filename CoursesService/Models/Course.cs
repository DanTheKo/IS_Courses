using CSharpFunctionalExtensions;

namespace CourseService.Models
{
    public class Course : Entity<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }


        //public Guid CourseMetadataId { get; set; }
        public CourseMetadata CourseMetadata { get; set; }

        public ICollection<CourseItem> CourseItems { get; set; } = new List<CourseItem>();

        public Course(Guid id,string title, string description, CourseMetadata courseMetadata , List<CourseItem> courseItems)
        {
            Id = id;
            Title = title;
            Description = description;
            CourseMetadata = courseMetadata;
            CourseItems = courseItems;
        }
        public Course(Guid id)
        {
            Id = id;
        }
    }
}
