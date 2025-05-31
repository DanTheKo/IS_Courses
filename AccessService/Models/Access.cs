using CSharpFunctionalExtensions;

namespace AccessService.Models
{
    public class Access : Entity<Guid>
    {
        public Guid IdentityId { get; set; }
        public Guid ResourceId { get; set;}
        public string AccessData { get; set; } 

        public Access(Guid id)
        {
            Id = id;
            AccessData = "Denied";
        }

        public Access()
        {
            AccessData = "Denied";
        }
    }
}
