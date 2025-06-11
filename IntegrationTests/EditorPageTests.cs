using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using FluentAssertions;
using Xunit;
using GatewayAPI;
using GatewayAPI.Services;
using Microsoft.AspNetCore.TestHost;
using GatewayAPI.Grpc;
using Microsoft.Extensions.DependencyInjection;
using GatewayAPI.RabbitMq;
using System.Net.Http.Json;

namespace IntegrationTests
{
    public class EditorPageTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public EditorPageTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }
        [Fact]
        public async Task OnGet_ValidCourseId_ReturnsPageWithData()
        {
            // Arrange
            var courseId = "course-123";
            var courseItemId = "item-456";
            var expectedCourse = new Course { Id = courseId, Title = "newtitle" };
            var expectedCourseItem = new CourseItem { Id = courseItemId, Type = "Testo", Title = "Module" };
            var expectedContent = new Content { Id = "content-789", Type = "Base", Data = "text..." };

            var rabbitMqMock = new Mock<IRabbitMqService>();
            var fakeServiceUrl = "http://localhost"; // или тестовая URL

            // Создаем реальный клиент, но подменяем его зависимости
            var courseClient = new Mock<CourseServiceClient>(fakeServiceUrl, rabbitMqMock.Object)
            {
                CallBase = true // Позволяет частично мокировать
            };

            // Подменяем только нужные методы
            courseClient
                .Setup(x => x.GetCourseAsync(courseId))
                .ReturnsAsync(expectedCourse);

            courseClient
                .Setup(x => x.GetCourseItemAsync(courseItemId))
                .ReturnsAsync(expectedCourseItem);

            courseClient
                .Setup(x => x.GetContentAsync("content-789"))
                .ReturnsAsync(expectedContent);

            var client = _factory
                .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(courseClient.Object);
                }))
                .CreateClient();


            // Act
            var response = await client.GetAsync($"/courses/editor/{courseId}/{courseItemId}");

            // Assert
            response.EnsureSuccessStatusCode(); // 200 OK
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain(expectedCourse.Title); // Проверяем, что данные отобразились
        }

    }


}