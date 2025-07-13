using CourseService.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories
{
    public class CourseRepository : BaseRepository<Course>
    {
        public CourseRepository(DbContext context) : base(context) { }

        public async Task<Course> GetWithItemsAsync(Guid id)
        {
            Course? course = await _context.Set<Course>()
                .Include(c => c.CourseItems)
                .ThenInclude(ci => ci.Children)
                .Include(c => c.CourseItems)
                .ThenInclude(ci => ci.Contents)
                .Include(c => c.CourseMetadata)
                .FirstOrDefaultAsync(c => c.Id == id);
            return course;
        }
        public async Task<IEnumerable<Course>> GetAllWithItemsAsync(int skip = 0, int take = 10)
        {
            return await _dbSet.Skip(skip).Take(take).Include(c => c.CourseMetadata).ToListAsync();
        }
    }
}