using CSharpFunctionalExtensions;

namespace IdentityService.Models
{
    public class Profile : Entity<Guid>
    {
        public Identity Identity { get; set; }
        public Guid IdentityId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName{ get; set; } = string.Empty;
        public string Status{ get; set; } = string.Empty;
        public string ProfileImageUrl{ get; set; } = string.Empty;

        public Profile(string id, string identityId, string firstName, string lastName, string middleName, string status, string profileImageUrl)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Id = Guid.NewGuid();
            }
            else
            {
                Id = Guid.Parse(id);
            }
            IdentityId = Guid.Parse(identityId);
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            Status = status;
            ProfileImageUrl = profileImageUrl;
        }
        public Profile()
        {
            
        }


    }
}
