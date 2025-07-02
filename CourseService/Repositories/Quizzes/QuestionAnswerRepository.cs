using CourseService.Models.Quizzes;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories.Quizzes
{
    public class QuestionAnswerRepository : BaseRepository<QuestionAnswer>
    {
        public QuestionAnswerRepository(DbContext context) : base(context) { }
    }
}
