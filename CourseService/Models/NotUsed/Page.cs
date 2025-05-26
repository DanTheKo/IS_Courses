using CSharpFunctionalExtensions;

namespace CourseService.Models.NotUsed
{
    public class Page : Entity<Guid>
    {
        public string Title { get; set; }
        public int Order { get; set; }

        public int ModuleId { get; set; }
        public Module Module { get; set; }

        public List<Content> Contents { get; set; } = new(); 

        public Page(string title, int order, int moduleId, Module module, List<Content> contents)
        {
            Id = Guid.NewGuid();
            Title = title;
            Order = order;
            ModuleId = moduleId;
            Module = module;
            Contents = contents;
        }
    }
}
