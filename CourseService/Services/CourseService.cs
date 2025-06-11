using CourseService.Data;
using CourseService.Models;
using CourseService.Repositories;
using CourseService.Grpc;
using Grpc.Core;
using CSharpFunctionalExtensions;

namespace CourseService.Services
{
    public class CourseService : Grpc.Courses.CoursesBase
    {
        private readonly ILogger<CourseService> _logger;

        private readonly CourseRepository _courseRepository;
        private readonly CourseItemRepository _courseItemRepository;
        private readonly ContentRepository _contentRepository;

        public CourseService(ILogger<CourseService> logger, CourseDbContext context)
        {
            _logger = logger;
            _courseRepository = new CourseRepository(context);
            _courseItemRepository = new CourseItemRepository(context);
            _contentRepository = new ContentRepository(context);
        }

        public override async Task<Grpc.Course> CreateCourse(CourseRequest request, ServerCallContext context)
        {
            Guid courseId = Guid.NewGuid();
            var course = new Models.Course(courseId);
            course.Title = request.Title;
            course.Description = request.Description;
            course.CourseMetadata = new Models.CourseMetadata()
            {
                Duration = TimeSpan.Parse(request.CourseMetadata.Duration),
                IsDeleted = request.CourseMetadata.IsDeleted,
                PreviewImageUrl = request.CourseMetadata.Image
            };

            await _courseRepository.AddAsync(course);
            return CourseServiceMapper.ToGrpcCourse(course);
        }

        public override async Task<Grpc.Course> GetCourse(GetByIdRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Trying to get course {request.Id}");
            var course = await _courseRepository.GetWithItemsAsync(Guid.Parse(request.Id));
            if (course == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Course with id {request.Id} not found"));
            }
            return CourseServiceMapper.ToGrpcCourse(course);
        }

        public override async Task<Grpc.PaginatedResponseCourse> GetCourses(PaginationRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Trying to get {request.PageSize} courses");
            var courses = await _courseRepository.GetAllAsync(request.PageNumber * request.PageSize, request.PageSize);
            PaginatedResponseCourse responce = new PaginatedResponseCourse();
            responce.TotalCount = courses.Count();
            responce.PageNumber = responce.PageNumber;
            responce.PageSize = request.PageSize;
            foreach (var course in courses)
            {
                responce.Items.Add(CourseServiceMapper.ToGrpcCourse(course));
            }
            return responce;
        }

        public override async Task<Grpc.Course> UpdateCourse(CourseRequest request, ServerCallContext context)
        {
            var course = await _courseRepository.GetByIdAsync(Guid.Parse(request.Id));
            if (course == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Course with id {request.Id} not found"));
            }
            course.Title = request.Title;
            course.Description = request.Description;
            if(request.CourseMetadata != null)
            {
                if (request.CourseMetadata.Duration != null) course.CourseMetadata.Duration = TimeSpan.Parse(request.CourseMetadata.Duration);
                course.CourseMetadata.IsDeleted = request.CourseMetadata.IsDeleted;
                if (request.CourseMetadata.Image != null) course.CourseMetadata.PreviewImageUrl = request.CourseMetadata.Image;

            }

            await _courseRepository.UpdateAsync(course);
            return CourseServiceMapper.ToGrpcCourse(course);
        }

        public override async Task<Empty> DeleteCourse(GetByIdRequest request, ServerCallContext context)
        {
            var course = await _courseRepository.GetByIdAsync(Guid.Parse(request.Id));
            if (course != null)
            {
                await _courseRepository.DeleteAsync(course.Id);
            }
            return new Empty();
        }

        public override async Task<Grpc.CourseItem> CreateCourseItem(CourseItemRequest request, ServerCallContext context)
        {

            var courseItem = new Models.CourseItem(Guid.NewGuid())
            {
                CourseId = Guid.Parse(request.CourseId),
                ParentId = null,
                Type = request.Type,
                Title = request.Title,
                Order = request.Order
            };

            await _courseItemRepository.AddAsync(courseItem);
            return CourseServiceMapper.ToGrpcCourseItem(courseItem);
        }

        public override async Task<Grpc.CourseItem> GetCourseItem(GetByIdRequest request, ServerCallContext context)
        {
            Console.WriteLine(request.Id);
            var courseItem = await _courseItemRepository.GetWithChildrenAsync(Guid.Parse(request.Id));
            if (courseItem == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"CourseItem with id {request.Id} not found"));
            }

            return CourseServiceMapper.ToGrpcCourseItem(courseItem);
        }

        public override async Task<Grpc.CourseItem> UpdateCourseItem(CourseItemRequest request, ServerCallContext context)
        {
            var courseItem = await _courseItemRepository.GetByIdAsync(Guid.Parse(request.Id));
            if (courseItem == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"CourseItem with id {request.Id} not found"));
            }

            courseItem.Title = request.Title;
            courseItem.Type = request.Type;

            await _courseItemRepository.UpdateAsync(courseItem);
            return CourseServiceMapper.ToGrpcCourseItem(courseItem);
        }

        public override async Task<Empty> DeleteCourseItem(GetByIdRequest request, ServerCallContext context)
        {
            var courseItem = await _courseItemRepository.GetByIdAsync(Guid.Parse(request.Id));
            if (courseItem != null)
            {
                await _courseItemRepository.DeleteAsync(courseItem.Id);
            }
            return new Empty();
        }

        public override async Task<Grpc.Content> CreateContent(ContentRequest request, ServerCallContext context)
        {
            var content = new Models.Content(Guid.NewGuid())
            {
                CourseItemId = Guid.Parse(request.CourseItemId),
                Type = request.Type,
                Data = request.Data,
                Order = request.Order
            };

            await _contentRepository.AddAsync(content);
            return CourseServiceMapper.ToGrpcContent(content);
        }

        public override async Task<Grpc.Empty> CreateContents(ContentsRequest contentsRequest, ServerCallContext context)
        {
            List<Models.Content> contents = new List<Models.Content>();
            foreach (var content in contentsRequest.ContentRequests)
            {
                var modelContent = new Models.Content(Guid.NewGuid())
                {
                    CourseItemId = Guid.Parse(content.CourseItemId),
                    Type = content.Type,
                    Data = content.Data,
                    Order = content.Order
                };
                contents.Add(modelContent);
            }

            await _contentRepository.AddRangeAsync(contents);
            return new Empty();
        }

        public override async Task<Grpc.Content> GetContent(GetByIdRequest request, ServerCallContext context)
        {
            var content = await _contentRepository.GetByIdAsync(Guid.Parse(request.Id));
            if (content == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Content with id {request.Id} not found"));
            }
            return CourseServiceMapper.ToGrpcContent(content);
        }

        public override async Task<Grpc.Content> UpdateContent(ContentRequest request, ServerCallContext context)
        {
            var content = await _contentRepository.GetByIdAsync(Guid.Parse(request.Id));
            if (content == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Content with id {request.Id} not found"));
            }
            content.Type = request.Type;
            content.Data = request.Data;

            await _contentRepository.UpdateAsync(content);
            return CourseServiceMapper.ToGrpcContent(content);
        }

        public override async Task<Empty> DeleteContent(GetByIdRequest request, ServerCallContext context)
        {
            var content = await _contentRepository.GetByIdAsync(Guid.Parse(request.Id));
            if (content != null)
            {
                await _contentRepository.DeleteAsync(content.Id);
            }
            return new Empty();
        }



    }


    public static class CourseServiceMapper
    {
        public static Grpc.Course ToGrpcCourse(Models.Course course)
        {
            var response = new Grpc.Course();
            response.Id = course.Id.ToString();
            response.Title = course.Title;
            response.Description = course.Description;
            response.CourseItemsIds.Add(course.CourseItems.Select(i => i.Id.ToString()));
            return response;
        }
        public static Grpc.Course ToCourse(Models.Course course)
        {
            var response = new Grpc.Course();
            response.Id = course.Id.ToString();
            response.Title = course.Title;
            response.Description = course.Description;
            response.CourseItemsIds.Add(course.CourseItems.Select(i => i.Id.ToString()));
            return response;
        }
        public static Grpc.CourseItem ToGrpcCourseItem(Models.CourseItem courseItem)
        {
            var response = new Grpc.CourseItem();
            response.Id = courseItem.Id.ToString();
            response.CourseId = courseItem.CourseId.ToString();
            response.Type = courseItem.Type;
            response.Title = courseItem.Title;
            response.Order = courseItem.Order;
            response.ChildrenIds.Add(courseItem.Children.Select(i => i.Id.ToString()));
            response.ContentsIds.Add(courseItem.Contents.Select(i => i.Id.ToString()));
            return response;
        }
        public static Grpc.CourseItem ToCourseItem(Models.CourseItem courseItem)
        {
            var response = new Grpc.CourseItem();
            response.Id = courseItem.Id.ToString();
            response.CourseId = courseItem.CourseId.ToString();
            response.Type = courseItem.Type;
            response.Title = courseItem.Title;
            response.Order = courseItem.Order;
            response.ChildrenIds.Add(courseItem.Children.Select(i => i.Id.ToString()));
            response.ContentsIds.Add(courseItem.Contents.Select(i => i.Id.ToString()));
            return response;
        }

        public static Grpc.Content ToGrpcContent(Models.Content content)
        {
            var response = new Grpc.Content();
            response.Id = content.Id.ToString();
            response.Order = content.Order;
            response.CourseItemId = content.Id.ToString();
            response.Type = content.Type;
            response.Data = content.Data;
            return response;
        }
        public static Grpc.Content ToContent(Models.Content content)
        {
            var response = new Grpc.Content();
            response.Id = content.Id.ToString();
            response.Order = content.Order;
            response.CourseItemId = content.Id.ToString();
            response.Type = content.Type;
            response.Data = content.Data;
            return response;
        }
    }
}
