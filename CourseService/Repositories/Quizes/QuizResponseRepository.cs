using CourseService.Models.Quizes;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories.Quizes
{
    public class QuizResponseRepository : BaseRepository<QuizResponse>
    {
        public QuizResponseRepository(DbContext context) : base(context) { }
    }
}
