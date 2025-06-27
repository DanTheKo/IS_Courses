using CSharpFunctionalExtensions;

namespace CourseService.Models.Interactivities
{
    public class InteractiveResponce : Entity<Guid>
    {
        public Guid InteractiveId { get; set; }
        public Guid IdentityId { get; set; }
    }
}
