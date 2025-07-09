using CourseService.Models.Quizzes;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories.Quizzes
{
    public class QuestionRepository : BaseRepository<Question>
    {
        public QuestionRepository(DbContext context) : base(context) { }

        public async Task<Question> GetWithChildrenAsync(Guid id)
        {
            Question? question = await _context.Set<Question>()
                .Include(e => e.QuestionAnswers)
                .FirstOrDefaultAsync(e => e.Id == id);
            return question;
        }

/*        public async Task<Question> GetWithChildrenByQuizIdAsync(Guid id)
        {
            Question? question = await _context.Set<Question>()
                .Include(e => e.QuestionAnswers)
                .FirstOrDefaultAsync(e => e.Id == id);
            return question;
        }*/

    }
}
