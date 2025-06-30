using CourseService.Models.Quizes;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories.Quizes
{
    public class QuestionRepository : BaseRepository<Question>
    {
        public QuestionRepository(DbContext context) : base(context) { }
    }
}
