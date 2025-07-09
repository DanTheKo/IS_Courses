using CourseService.Models.Quizzes;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories.Quizzes
{
    public class QuizResponseRepository : BaseRepository<QuizResponse>
    {
        public QuizResponseRepository(DbContext context) : base(context) { }

        public async Task<QuizResponse> GetWithChildrenAsync(Guid id)
        {
            QuizResponse? quizResponse = await _context.Set<QuizResponse>()
                .Include(e => e.QuestionAnswers)
                .FirstOrDefaultAsync(e => e.Id == id);
            return quizResponse;
        }
    }
}
