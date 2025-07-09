using CourseService.Models.Quizzes;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories.Quizzes
{
    public class QuestionAnswerRepository : BaseRepository<QuestionAnswer>
    {
        public QuestionAnswerRepository(DbContext context) : base(context) { }

        public async Task<QuestionAnswer> GetWithChildrenAsync(Guid id)
        {
            QuestionAnswer? question = await _context.Set<QuestionAnswer>()
                .Include(e => e.Feedbacks)
                .FirstOrDefaultAsync(e => e.Id == id);
            return question;
        }
    }
}
