namespace GatewayAPI.Models.DTO
{
    public class CourseDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public string PreviewImageUrl { get; set; }
        public string Duration { get; set; }
    }
}
