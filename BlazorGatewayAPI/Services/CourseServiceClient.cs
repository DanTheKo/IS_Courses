﻿using Grpc.Core;
using Grpc.Net.Client;
using BlazorGatewayAPI.Grpc;

namespace BlazorGatewayAPI.Services
{
    public class CourseServiceClient : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly Courses.CoursesClient _client;

        public CourseServiceClient(string serviceUrl)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Service URL cannot be null or empty", nameof(serviceUrl));

            _channel = GrpcChannel.ForAddress(serviceUrl);
            _client = new Courses.CoursesClient(_channel);
        }


        public async Task<Course> CreateCourseAsync(string title, string description)
        {
            try
            {
                var request = new CourseRequest
                {
                    Title = title,
                    Description = description,
                    CourseMetadata = new CourseMetadataRequest()
                    {
                        Duration = TimeSpan.FromHours(10d).ToString(),
                        IsDeleted = false,
                        Image = "пикча"
                    }
                };
                return await _client.CreateCourseAsync(request);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to create course: {ex.Status.Detail}", ex);
            }
        }

        public async Task<Course> GetCourseAsync(string id)
        {
            try
            {
                var request = new GetByIdRequest { Id = id };
                return await _client.GetCourseAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to get course: {ex.Status.Detail}", ex);
            }
        }

        public async Task<Course> UpdateCourseAsync(string id, string title, string description)
        {
            try
            {
                var request = new CourseRequest
                {
                    Id = id,
                    Title = title,
                    Description = description
                };
                return await _client.UpdateCourseAsync(request);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to update course: {ex.Status.Detail}", ex);
            }
        }

        public async Task<bool> DeleteCourseAsync(string id)
        {
            try
            {
                var request = new GetByIdRequest { Id = id };
                await _client.DeleteCourseAsync(request);
                return true;
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to delete course: {ex.Status.Detail}", ex);
            }
        }

        // Элементы курса
        public async Task<CourseItem> CreateCourseItemAsync(string courseId, string parentId, string type, string title)
        {
            try
            {
                var request = new CourseItemRequest
                {
                    CourseId = courseId,
                    ParentId = parentId,
                    Type = type,
                    Title = title
                };
                return await _client.CreateCourseItemAsync(request);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to create course item: {ex.Status.Detail}", ex);
            }
        }

        public async Task<CourseItem> GetCourseItemAsync(string id)
        {
            try
            {
                var request = new GetByIdRequest { Id = id };
                return await _client.GetCourseItemAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to get course item: {ex.Status.Detail}", ex);
            }
        }

        public async Task<CourseItem> UpdateCourseItemAsync(string id, string title, string type)
        {
            try
            {
                var request = new CourseItemRequest
                {
                    Id = id,
                    Title = title,
                    Type = type
                };
                return await _client.UpdateCourseItemAsync(request);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to update course item: {ex.Status.Detail}", ex);
            }
        }

        public async Task<bool> DeleteCourseItemAsync(string id)
        {
            try
            {
                var request = new GetByIdRequest { Id = id };
                await _client.DeleteCourseItemAsync(request);
                return true;
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to delete course item: {ex.Status.Detail}", ex);
            }
        }

        // Контент
        public async Task<Content> CreateContentAsync(string courseItemId, string type, string data)
        {
            try
            {
                var request = new ContentRequest
                {
                    CourseItemId = courseItemId,
                    Type = type,
                    Data = data
                };
                return await _client.CreateContentAsync(request);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to create content: {ex.Status.Detail}", ex);
            }
        }

        public async Task<Content> GetContentAsync(string id)
        {
            try
            {
                var request = new GetByIdRequest { Id = id };
                return await _client.GetContentAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to get content: {ex.Status.Detail}", ex);
            }
        }

        public async Task<Content> UpdateContentAsync(string id, string type, string data)
        {
            try
            {
                var request = new ContentRequest
                {
                    Id = id,
                    Type = type,
                    Data = data
                };
                return await _client.UpdateContentAsync(request);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to update content: {ex.Status.Detail}", ex);
            }
        }

        public async Task<bool> DeleteContentAsync(string id)
        {
            try
            {
                var request = new GetByIdRequest { Id = id };
                await _client.DeleteContentAsync(request);
                return true;
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to delete content: {ex.Status.Detail}", ex);
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
