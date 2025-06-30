using CourseService.Models.Quizes;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories.Quizes
{
    public class QuizRepository : BaseRepository<Quiz>
    {
        public QuizRepository(DbContext context) : base(context) { }
    }
}
