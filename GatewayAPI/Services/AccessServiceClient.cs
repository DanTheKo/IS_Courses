using GatewayAPI.Grpc;
using Grpc.Core;
using Grpc.Net.Client;

namespace GatewayAPI.Services
{
    public class AccessServiceClient : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly Accesses.AccessesClient _client;

        public AccessServiceClient(string serviceUrl)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Service URL cannot be null or empty", nameof(serviceUrl));

            _channel = GrpcChannel.ForAddress(serviceUrl);
            _client = new Accesses.AccessesClient(_channel);
        }

        public async Task<AccessResponce> CreateAccessAsync(string identityId, string resourceId, string accessData)
        {
            try
            {
                AccessDataRequest request = new AccessDataRequest();
                request.IdentityId = identityId;
                request.ResourceId = resourceId;
                request.AccessData = accessData;
                return await _client.CreateAccessAsync(request);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to create: {ex.Status.Detail}", ex);
            }
        }

        public async Task<AccessResponce> UpdateAccessAsync(string identityId, string resourceId, string accessData)
        {
            try
            {
                AccessDataRequest request = new AccessDataRequest();
                request.IdentityId = identityId;
                request.ResourceId = resourceId;
                request.AccessData = accessData;
                return await _client.UpdateAccessAsync(request);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to update: {ex.Status.Detail}", ex);
            }
        }
        public async Task<AccessResponce> GetAccessAsync(string identityId, string resourceId)
        {
            try
            {
                GetAccessByIdsRequest request = new GetAccessByIdsRequest();
                request.IdentityId = identityId;
                request.ResourceId = resourceId;
                return await _client.GetAccessAsync(request);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to get: {ex.Status.Detail}", ex);
            }
        }

        public async Task<EmptyAccessResponce> DeleteAccessAsync(string identityId, string resourceId)
        {
            try
            {
                GetAccessByIdsRequest request = new GetAccessByIdsRequest();
                request.IdentityId = identityId;
                request.ResourceId = resourceId;

                return await _client.DeleteAccessAsync(request);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Failed to delete: {ex.Status.Detail}", ex);
            }
        }


        public void Dispose()
        {
            _channel?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
