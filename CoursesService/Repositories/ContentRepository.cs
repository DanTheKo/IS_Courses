using CourseService.Data;
using CourseService.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories
{
    public class ContentRepository : BaseRepository<Content>
    {
        public ContentRepository(DbContext context) : base(context) { }

        public async Task<IEnumerable<Content>> GetByCourseItemAsync(Guid courseItemId)
        {
            return await _context.Set<Content>()
                .Where(c => c.CourseItem.Id.Equals(courseItemId))
                .OrderBy(c => c.Order)
                .ToListAsync();
        }

        public async Task<int> GetLastOrderAsync(Guid courseItemId)
        {
            CourseDbContext dbContext = (CourseDbContext)_context;
            return await dbContext.Contents
                .Where(x => x.CourseItem.Id.Equals(courseItemId))
                .OrderByDescending(x => x.Order)
                .Select(x => x.Order)
                .FirstOrDefaultAsync();
        }
    }
}
