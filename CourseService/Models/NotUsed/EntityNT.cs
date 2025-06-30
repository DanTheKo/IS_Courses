using System;
namespace CourseService.Models.NotUsed
{
    public abstract class EntityNT<TId>
    {
        public virtual TId Id { get;  set; }

        protected EntityNT()
        {
        }

        protected EntityNT(TId id)
        {
            Id = id;
        }
    }
}
