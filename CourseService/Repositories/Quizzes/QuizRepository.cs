using CourseService.Models.Quizzes;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories.Quizzes
{
    public class QuizRepository : BaseRepository<Quiz>
    {
        public QuizRepository(DbContext context) : base(context) { }
    }
}
