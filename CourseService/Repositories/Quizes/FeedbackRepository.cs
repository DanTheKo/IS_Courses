using CourseService.Models.Quizes;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories.Quizes
{
    public class FeedbackRepository :BaseRepository<Feedback>
    {
        public FeedbackRepository(DbContext context) : base(context) { }
    }
}
