using CourseService.Models.Quizzes;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories.Quizzes
{
    public class FeedbackRepository :BaseRepository<Feedback>
    {
        public FeedbackRepository(DbContext context) : base(context) { }
    }
}
