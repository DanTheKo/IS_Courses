using CSharpFunctionalExtensions;

namespace CourseService.Models
{
    public class Content : Entity<Guid>
    {
        public string Type { get; set; }
        public string Data { get; set; }
        public int Order { get; set; }

        public Guid CourseItemId { get; set; }
        public CourseItem CourseItem { get; set; }

        public Content(Guid id, string type, string data, int order, Guid courseItemId,  CourseItem courseItem)
        {
            Id = id;
            Type = type;
            Data = data;
            Order = order;
            CourseItemId = courseItemId;
            CourseItem = courseItem;
        }
        public Content(Guid id)
        {
            Id = id;
        }
    }

}
