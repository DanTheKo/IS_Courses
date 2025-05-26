using CourseService.Data;
using CourseService.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Repositories
{
    public class CourseItemRepository : BaseRepository<CourseItem>
    {
        public CourseItemRepository(DbContext context) : base(context) { }

        public async Task<CourseItem> GetWithChildrenAsync(Guid id)
        {
            CourseItem? courseItem = await _context.Set<CourseItem>()
                .Include(ci => ci.Children)
                .Include(ci => ci.Contents)
                .FirstOrDefaultAsync(ci => ci.Id == id);
            return courseItem;
        }

        public async Task AddContentAsync(Guid itemId, Content content)
        {
            var item = await GetByIdAsync(itemId);
            if (item != null)
            {
                item.Contents.Add(content);
                await UpdateAsync(item);
            }
        }

        public async Task<int> GetLastOrderAsync(Guid courseId, Guid? parentId)
        {
            CourseDbContext dbContext = (CourseDbContext)_context;
            return await dbContext.CourseItems
                .Where(x => x.Course.Id.Equals(courseId) && x.Parent.Id.Equals(parentId))
                .OrderByDescending(x => x.Order)
                .Select(x => x.Order)
                .FirstOrDefaultAsync();
        }
    }
}
