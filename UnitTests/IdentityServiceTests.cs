using IdentityService.Data;
using ISS = IdentityService.Services ;
using IdentityService.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using IdentityService.Models;
using IdentityService;
using FluentAssertions;

namespace UnitTests
{
    public class IdentityServiceTests
    {
        private readonly Mock<ILogger<ISS.IdentityService>> _loggerMock;
        private readonly Mock<IdentityDbContext> _dbContextMock;
        private readonly Mock<IdentityRepository> _identityRepositoryMock;
        private readonly ISS.IdentityService _identityService;

        public IdentityServiceTests()
        {
            _loggerMock = new Mock<ILogger<ISS.IdentityService>>();
            _dbContextMock = new Mock<IdentityDbContext>();
            _identityRepositoryMock = new Mock<IdentityRepository>(_dbContextMock.Object);

            // Создаем экземпляр IdentityService с моком репозитория
            _identityService = new ISS.IdentityService(_loggerMock.Object, _dbContextMock.Object)
            {
                // Заменяем реальный репозиторий на мок
                _identityRepository = _identityRepositoryMock.Object
            };
        }

        public class Register : IdentityServiceTests
        {
            [Fact]
            public async Task ShouldReturnSuccess_WhenRegistrationIsSuccessful()
            {
                // Arrange
                var request = new RegistrationRequest
                {
                    Username = "testuser",
                    Email = "test@example.com",
                    Password = "password123",
                    Phone = "1234567890"
                };

                _identityRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Username))
                    .ReturnsAsync((Identity)null);
                _identityRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email))
                    .ReturnsAsync((Identity)null);
                _identityRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Identity>()))
                    .Returns(Task.CompletedTask);

                // Act
                var response = await _identityService.Register(request, null);

                // Assert
                response.Should().NotBeNull();
                response.Success.Should().BeTrue();
                response.UserId.Should().NotBeNullOrEmpty();
                response.Errors.Should().BeEmpty();
            }


        }

        public class Authenticate : IdentityServiceTests
        {
            [Fact]
            public async Task ShouldReturnSuccess_WhenCredentialsAreValid()
            {
                // Arrange
                var request = new AuthenticationRequest
                {
                    Login = "testuser",
                    Password = "correctpassword"
                };

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
                var identity = new Identity(Guid.NewGuid())
                {   
                    Login = "testuser",
                    Password = hashedPassword,
                    Role = "User"
                };

                _identityRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Login))
                    .ReturnsAsync(identity);

                // Act
                var response = await _identityService.Authenticate(request, null);

                // Assert
                response.Should().NotBeNull();
                response.Success.Should().BeTrue();
                response.UserId.Should().Be(identity.Id.ToString());
                response.Role.Should().Be(identity.Role);
                response.Error.Should().BeNullOrEmpty();
            }
        }
        public class GetUserInfo : IdentityServiceTests
        {
            [Fact]
            public async Task ShouldReturnUserInfo_WhenUserExists()
            {
                // Arrange
                var userId = Guid.NewGuid();
                var request = new UserInfoRequest { UserId = userId.ToString() };

                var identity = new Identity(Guid.NewGuid())
                {
                    Login = "testuser",
                    Email = "test@example.com",
                    Phone = "1234567890",
                    Role = "User"
                };

                _identityRepositoryMock.Setup(x => x.GetByIdAsync(userId))
                    .ReturnsAsync(identity);

                // Act
                var response = await _identityService.GetUserInfo(request, null);

                // Assert
                response.Should().NotBeNull();
                response.Username.Should().Be(identity.Login);
                response.Email.Should().Be(identity.Email);
                response.Phone.Should().Be(identity.Phone);
                response.Role.Should().Be(identity.Role);
            }

            [Fact]
            public async Task ShouldBeNull_WhenUserNotFound()
            {
                // Arrange
                var userId = Guid.NewGuid();
                var request = new UserInfoRequest { UserId = userId.ToString() };

                _identityRepositoryMock.Setup(x => x.GetByIdAsync(userId))
                    .ReturnsAsync((Identity)null);

                // Act & Assert
                var responce = await _identityService.GetUserInfo(request, null);
                responce.Should().Be(null);
            }
        }
    }
}
