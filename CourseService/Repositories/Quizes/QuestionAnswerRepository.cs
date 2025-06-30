using CourseService.Models.Quizes;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories.Quizes
{
    public class QuestionAnswerRepository : BaseRepository<QuestionAnswer>
    {
        public QuestionAnswerRepository(DbContext context) : base(context) { }
    }
}
