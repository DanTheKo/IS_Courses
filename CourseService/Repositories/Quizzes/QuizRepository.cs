using CourseService.Models.Quizzes;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories.Quizzes
{
    public class QuizRepository : BaseRepository<Quiz>
    {
        public QuizRepository(DbContext context) : base(context) { }

        public async Task<Quiz> GetWithChildrenAsync(Guid id)
        {
            Quiz? quiz = await _context.Set<Quiz>()
                .Include(e => e.Questions)
                .Include(e => e.QuizResponses)
                .FirstOrDefaultAsync(e => e.Id == id);
            return quiz;
        }
    }
}
