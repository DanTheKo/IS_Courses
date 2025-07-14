using IdentityService.Data;
using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Repositories
{
    public class ProfileRepository : BaseRepository<Profile>
    {
        public ProfileRepository(DbContext context) : base(context) { }

        public virtual async Task<Profile?> GetByIdentityAsync(string id)
        {
            return await _context.Set<Profile>().FirstOrDefaultAsync(e => e.IdentityId.ToString() == id);
        }
    }
}
