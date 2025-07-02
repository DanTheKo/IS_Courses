using CourseService.Models.Quizzes;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories.Quizzes
{
    public class QuestionRepository : BaseRepository<Question>
    {
        public QuestionRepository(DbContext context) : base(context) { }
    }
}
