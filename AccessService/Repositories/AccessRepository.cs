using AccessService.Models;
using Microsoft.EntityFrameworkCore;

namespace AccessService.Repositories
{
    public class AccessRepository : BaseRepository<Access>
    {
        public AccessRepository(DbContext context) : base(context) { }

        public async Task<IEnumerable<Access>> GetAllByIdentityIdAsync(Guid identityId)
        {
            return await _context.Set<Access>()
                .Where(c => c.IdentityId == identityId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Access>> GetAllByResourceIdAsync(Guid resourceId)
        {
            return await _context.Set<Access>()
                .Where(c => c.ResourceId == resourceId)
                .ToListAsync();
        }

        public async Task<Access> GetByResourceId_IdentityIdAsync(Guid identityId, Guid resourceId)
        {
            Access? access = await _context.Set<Access>()
                .Where(c => c.IdentityId == identityId && c.ResourceId == resourceId)
                .FirstOrDefaultAsync();
            if (access != null)
            {
                return access;
            }
            else
            {
                access = new Access();
                access.IdentityId = identityId;
                access.ResourceId = resourceId;
                return access;
            }
            
        }
    }
}
