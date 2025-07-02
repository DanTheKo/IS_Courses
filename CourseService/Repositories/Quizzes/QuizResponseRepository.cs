using CourseService.Models.Quizzes;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories.Quizzes
{
    public class QuizResponseRepository : BaseRepository<QuizResponse>
    {
        public QuizResponseRepository(DbContext context) : base(context) { }
    }
}
