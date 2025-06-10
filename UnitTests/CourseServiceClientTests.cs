using FluentAssertions;
using GatewayAPI.Grpc;
using GatewayAPI.RabbitMq;
using GatewayAPI.Services;
using Grpc.Core;
using Grpc.Net.Client;
using Moq;

namespace UnitTests
{
    public class CourseServiceClientTests
    {

        private readonly Mock<IRabbitMqService> _rabbitMqMock = new();

        [Fact]
        public void Constructor_InvalidUrl_ThrowsArgumentException()
        {
            var invalidUrl = "";
            var rabbitMq = _rabbitMqMock.Object;

            Action act = () => new CourseServiceClient(invalidUrl, rabbitMq);
            act.Should().Throw<ArgumentException>()
                .WithMessage("Service URL cannot be null or empty*")
                .And.ParamName.Should().Be("serviceUrl");
        }
        [Fact]
        public async Task CreateCourseAsync_ValidRequest_ReturnsCourse()
        {
            var grpcClientMock = new Mock<Courses.CoursesClient>();
            var expectedCourse = new Course {Title = "Test", Description = "Description" };

            grpcClientMock
                .Setup(x => x.CreateCourseAsync(It.IsAny<CourseRequest>(), null, null, CancellationToken.None))
                .Returns(CreateAsyncUnaryCall<Course>(expectedCourse)); 

            var channel = GrpcChannel.ForAddress("http://localhost");
            var service = new CourseServiceClient("http://localhost", _rabbitMqMock.Object)
            {
                _client = grpcClientMock.Object
            };


            var result = await service.CreateCourseAsync("Test", "Description");


            result.Should().BeEquivalentTo(expectedCourse);
            grpcClientMock.Verify(x => x.CreateCourseAsync(It.IsAny<CourseRequest>(), null, null, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateCourseAsync_GrpcError_ThrowsWrappedException()
        {
            var grpcClientMock = new Mock<Courses.CoursesClient>();
            var status = new Status(StatusCode.Internal, "Server error");

            grpcClientMock
                .Setup(x => x.CreateCourseAsync(It.IsAny<CourseRequest>(), null, null, CancellationToken.None))
                .Returns(new AsyncUnaryCall<Course>(
                    Task.FromException<Course>(new RpcException(status)),
                    Task.FromResult(new Metadata()),
                    () => status,
                    () => new Metadata(),
                    () => { }));

            var service = new CourseServiceClient("http://localhost", _rabbitMqMock.Object)
            {
                _client = grpcClientMock.Object
            };

            var exception = await Assert.ThrowsAsync<Exception>(() => service.CreateCourseAsync("Test", "Desc"));

            exception.Message.Should().Be("Failed to create course: Server error");

            exception.InnerException.Should().BeOfType<RpcException>()
                .Which.StatusCode.Should().Be(StatusCode.Internal);
        }

        [Fact]
        public async Task GetCourseAsync_ValidId_ReturnsCourse()
        {
            var grpcClientMock = new Mock<Courses.CoursesClient>();
            var expectedCourse = new Course { Id = "1" };

            grpcClientMock
                .Setup(x => x.GetCourseAsync(It.IsAny<GetByIdRequest>(), null, null, CancellationToken.None))
                .Returns(CreateAsyncUnaryCall(expectedCourse));

            var service = new CourseServiceClient("http://localhost", _rabbitMqMock.Object)
            {
                _client = grpcClientMock.Object
            };

            var result = await service.GetCourseAsync("1");

            result.Should().BeEquivalentTo(expectedCourse);
        }
        [Fact]
        public async Task Dispose_ShouldDisposeGrpcChannel()
        {
            var channel = GrpcChannel.ForAddress("http://localhost");
            var service = new CourseServiceClient("http://localhost", _rabbitMqMock.Object)
            {
                _channel = channel
            };

            service.Dispose();

            await channel.Invoking(async c => await c.WaitForStateChangedAsync(c.State))
                .Should().ThrowAsync<ObjectDisposedException>();
        }

        public static AsyncUnaryCall<T> CreateAsyncUnaryCall<T>(T response)
        {
            return new AsyncUnaryCall<T>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
        }


    }


}
