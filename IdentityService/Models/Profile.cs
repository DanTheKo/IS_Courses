using CSharpFunctionalExtensions;

namespace IdentityService.Models
{
    public class Profile : Entity<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName{ get; set; } = string.Empty;
        public string Status{ get; set; } = string.Empty;
        public string ProfileImageUrl{ get; set; } = string.Empty;
        public Profile(Guid id)
        {
            Id = id;
        }
    }
}
