using CourseService.Grpc;
using CourseService.Models;
/*
namespace CourseService.Extensions
{
    public static class CourseMetadataExtensions
    {
        public static Grpc.CourseMetadata ToProto(this Models.CourseMetadata metadata)
        {
            return new Grpc.CourseMetadata
            {
                Id = metadata.Id.ToString(),
                IsDeleted = metadata.IsDeleted,
                PreviewImageUrl = metadata.PreviewImageUrl,
                Duration = metadata.Duration.ToString(),
                CourseId = metadata.CourseId.ToString()
            };
        }

        public static Models.CourseMetadata ToDomain(this Grpc.CourseMetadata proto)
        {
            return new Models.CourseMetadata(Guid.Parse(proto.Id))
            {
                IsDeleted = proto.IsDeleted,
                PreviewImageUrl = proto.PreviewImageUrl,
                Duration = TimeSpan.Parse(proto.Duration),
                CourseId = Guid.Parse(proto.CourseId)
            };
        }
    }

    // Conversion methods for CourseItem
    public static class CourseItemExtensions
    {
        public static Grpc.CourseItem ToProto(this Models.CourseItem item)
        {
            var proto = new Grpc.CourseItem
            {
                Id = item.Id.ToString(),
                Title = item.Title,
                Type = item.Type,
                Order = item.Order,
                //CourseItems = { item.Children.Select(c => c.ToProto()) },
                //Contents = { item.Contents.Select(c => c.ToProto()) }
            };

            if (item.ParentId.HasValue)
            {
                proto.ParentId = item.ParentId.Value.ToString();
            }

            if (item.Course != null)
            {
                //proto.Course = item.Course.ToProto();
            }

            return proto;
        }

        public static Models.CourseItem ToDomain(this Grpc.CourseItem proto)
        {
            var item = new Models.CourseItem(Guid.Parse(proto.Id))
            {
                Title = proto.Title,
                Type = proto.Type,
                Order = proto.Order
            };

            if (!string.IsNullOrEmpty(proto.ParentId))
            {
                item.ParentId = Guid.Parse(proto.ParentId);
            }

            // Note: Children and Course will be set by the calling code
            return item;
        }
    }

    // Conversion methods for Content
    public static class ContentExtensions
    {
        public static Grpc.Content ToProto(this Models.Content content)
        {
            return new Grpc.Content
            {
                Id = content.Id.ToString(),
                Type = content.Type,
                Data = content.Data,
                Order = content.Order,
                CourseItemId = content.CourseItemId.ToString()
            };
        }

        public static Models.Content ToDomain(this Grpc.Content proto)
        {
            return new Models.Content(Guid.Parse(proto.Id))
            {
                Type = proto.Type,
                Data = proto.Data,
                Order = proto.Order,
                CourseItemId = Guid.Parse(proto.CourseItemId)
            };
        }
    }

    // Conversion methods for Course
    public static class CourseExtensions
    {
        public static Grpc.Course ToProto(this Models.Course course)
        {
            return new Grpc.Course
            {
                Id = course.Id.ToString(),
                Title = course.Title,
                Description = course.Description,
                CourseMetadataId = course.CourseMetadataId.ToString(),
                CourseItems = { course.CourseItems.Select(ci => ci.ToProto()) }
            };
        }

        public static Models.Course ToDomain(this Grpc.Course proto)
        {
            var course = new Models.Course(Guid.Parse(proto.Id))
            {
                Title = proto.Title,
                Description = proto.Description,
                CourseMetadataId = Guid.Parse(proto.CourseMetadataId),
                CourseItems = proto.CourseItems.Select(ci => ci.ToDomain()).ToList()
            };

            // Set up parent-child relationships for course items
            var itemsById = course.CourseItems.ToDictionary(i => i.Id);
            foreach (var item in course.CourseItems)
            {
                if (item.ParentId.HasValue && itemsById.TryGetValue(item.ParentId.Value, out var parent))
                {
                    item.Parent = parent;
                    parent.Children.Add(item);
                }
            }

            return course;
        }
    }
}
*/