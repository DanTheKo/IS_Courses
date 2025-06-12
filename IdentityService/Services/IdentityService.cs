using Grpc.Core;
using IdentityService.Models;
using IdentityService.Repositories;
using IdentityService.Data;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Services
{
    public class IdentityService : Authorization.AuthorizationBase
    {
        public ILogger<IdentityService> _logger;
        public IdentityRepository _identityRepository;
        //private readonly ITokenService _tokenService;

        public IdentityService(ILogger<IdentityService> logger, IdentityDbContext context)
        {
            _logger = logger;
            _identityRepository = new IdentityRepository(context);
        }
        
        public override async Task<RegistrationResponse> Register(RegistrationRequest request, ServerCallContext context)
        {
            try
            {
                if (await _identityRepository.GetByUsernameAsync(request.Username) != null)
                {
                    return new RegistrationResponse
                    {
                        Success = false,
                        Errors = { "Username already exists" }
                    };
                }
                if (await _identityRepository.GetByEmailAsync(request.Email) != null)
                {
                    return new RegistrationResponse
                    {
                        Success = false,
                        Errors = { "Email already exists" }
                    };
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, BCrypt.Net.BCrypt.GenerateSalt(12));

                var identity = new Identity(Guid.NewGuid())
                {
                    Login = request.Username,
                    Email = request.Email,
                    Password = passwordHash,
                    Phone = request.Phone,
                    Role = "User"
                };

                await _identityRepository.AddAsync(identity);

                return new RegistrationResponse
                {
                    Success = true,
                    UserId = identity.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return new RegistrationResponse
                {
                    Success = false,
                    Errors = { "Internal server error" }
                };
            }
        }

        public override async Task<AuthenticationResponse> Authenticate(AuthenticationRequest request, ServerCallContext context)
        {
            try
            {
                var identity = await _identityRepository.GetByUsernameAsync(request.Login) ??
                          await _identityRepository.GetByEmailAsync(request.Login);

                if (identity == null || !BCrypt.Net.BCrypt.Verify(request.Password, identity.Password))
                {
                    return new AuthenticationResponse
                    {
                        Success = false,
                        Error = "Invalid credentials"
                    };
                }


                return new AuthenticationResponse
                {
                    Success = true,
                    Token = "Token",
                    UserId = identity.Id.ToString(),
                    Role = identity.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authentication failed");
                return new AuthenticationResponse
                {
                    Success = false,
                    Error = "Internal server error"
                };
            }
        }


        public override async Task<UserInfoResponse> GetUserInfo(UserInfoRequest request, ServerCallContext context)
        {
            try
            {
                Guid id;
                Guid.TryParse(request.UserId, out id);

                var identity = await _identityRepository.GetByIdAsync(id);
                
                if (identity == null)
                {
                    return null;
                }

                return new UserInfoResponse
                {
                    Username = identity.Login,
                    Email = identity.Email,
                    Phone = identity.Phone,
                    Role = identity.Role
                };
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user info");
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }
    }
}