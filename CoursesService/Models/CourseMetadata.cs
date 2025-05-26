using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Models
{
    [Owned]
    public class CourseMetadata
    {
        public bool IsDeleted { get; set; }
        public string PreviewImageUrl { get; set; }
        public TimeSpan Duration { get; set; }

        //public int AverageRating { get; set; }
        //public int TotalEnrollments { get; set; }

        //public string Language { get; set; } = "ru";
        //public string DifficultyLevel { get; set; }

    }
}
