using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;  
using Google.Protobuf;
using GatewayAPI.Grpc;

namespace GatewayAPI.Services
{
    public class AuthorizationServiceClient : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly Authorization.AuthorizationClient _client;

        public AuthorizationServiceClient(string serviceUrl)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Service URL cannot be null or empty", nameof(serviceUrl));

            _channel = GrpcChannel.ForAddress(serviceUrl);
            _client = new Authorization.AuthorizationClient(_channel);
        }

        public async Task<RegistrationResponse> RegisterAsync(
            string username,
            string password,
            string email,
            string? phone)
        {
            var request = new RegistrationRequest
            {
                Username = username,
                Password = password,
                Email = email,
                Phone = phone,
            };

            return await _client.RegisterAsync(request);
        }

        public async Task<AuthenticationResponse> AuthenticateAsync(string login, string password)
        {
            var request = new AuthenticationRequest
            {
                Login = login,
                Password = password
            };

            return await _client.AuthenticateAsync(request);
        }

        public async Task<TokenValidationResponse> ValidateTokenAsync(string token)
        {
            var request = new TokenValidationRequest
            {
                Token = token
            };

            return await _client.ValidateTokenAsync(request);
        }

        public async Task<UserInfoResponse> GetUserInfoAsync(string userId)
        {
            var request = new UserInfoRequest
            {
                UserId = userId
            };

            return await _client.GetUserInfoAsync(request);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}