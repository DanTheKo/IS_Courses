using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Repositories
{
    public class ProfileRepository : BaseRepository<Profile>
    {
        public ProfileRepository(DbContext context) : base(context) { }
    }
}
