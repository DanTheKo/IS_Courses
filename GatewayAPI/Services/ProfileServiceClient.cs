
using GatewayAPI.Grpc;
using Grpc.Net.Client;

namespace GatewayAPI.Services
{
    public class ProfileServiceClient : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly ProfileGrpcService.ProfileGrpcServiceClient _profileClient;

        public ProfileServiceClient(string serverUrl)
        {
            _channel = GrpcChannel.ForAddress(serverUrl);
            _profileClient = new ProfileGrpcService.ProfileGrpcServiceClient(_channel);

        }

        public void Dispose()
        {
            _channel?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<Profile> CreateProfileAsync(Profile profile)
        {
            var request = new CreateRequest
            {
                Profile = profile
            };

            var response = await _profileClient.CreateAsync(request);
            return response.Profile;
        }

        public async Task<Profile> UpdateProfileAsync(Profile profile)
        {
            var request = new UpdateRequest
            {
                Profile = profile
            };

            var response = await _profileClient.UpdateAsync(request);
            return response.Profile;
        }

        public async Task<Profile> GetProfileAsync(string id)
        {
            var request = new ReadRequest
            {
                EntityType = "profile",
                Id = id
            };

            var response = await _profileClient.ReadAsync(request);
            return response.Profile;
        }

        public async Task<Profile> GetProfileByIdentityIdAsync(string id)
        {
            var request = new ReadRequest
            {
                EntityType = "profile",
                Id = id
            };

            var response = await _profileClient.ReadByIdentityIdAsync(request);
            return response.Profile;
        }

        public async Task DeleteProfileAsync(string id)
        {
            var request = new DeleteRequest
            {
                EntityType = "profile",
                Id = id
            };

            await _profileClient.DeleteAsync(request);
        }

        public async Task<IEnumerable<Profile>> GetAllProfilesAsync()
        {
            var request = new ListRequest
            {
                EntityType = "profile"
            };

            var response = await _profileClient.ListAsync(request);
            return response.Entities.Select(e => e.Profile);
        }
    }
}
