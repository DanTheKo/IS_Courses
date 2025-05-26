using CSharpFunctionalExtensions;

namespace AccessService.Models
{
    public class Access : Entity<Guid>
    {
        public Guid IdentityId { get; set; }
        public Guid ItemId { get; set;}
        public string AccessType { get; set; }


    }
}
