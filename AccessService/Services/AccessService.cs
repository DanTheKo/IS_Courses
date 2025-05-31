using AccessService.Data;
using AccessService.Repositories;
using AccessService.Grpc;
using Grpc.Core;
using AccessService.Models;

namespace AccessService.Services
{
    public class AccessService : Grpc.Accesses.AccessesBase
    {
        private readonly ILogger<AccessService> _logger;

        private readonly AccessRepository _accessRepository;

        public AccessService(ILogger<AccessService> logger, AccessDbContext context)
        {
            _logger = logger;
            _accessRepository = new AccessRepository(context);
        }
        public override async Task<AccessResponce> CreateAccess(AccessDataRequest request, ServerCallContext context)
        {
            Guid accessId = Guid.NewGuid();
            Access access = new Access(accessId);
            access.IdentityId = Guid.Parse(request.IdentityId);
            access.ResourceId = Guid.Parse(request.ResourceId);
            access.AccessData = request.AccessData;
            await _accessRepository.AddAsync(access);

            return AccessServiceMapper.ToGrpcAccessResponce(access);

        }

        public override async Task<AccessResponce> GetAccess(GetAccessByIdsRequest request, ServerCallContext context)
        {
            Access access = await _accessRepository.GetByResourceId_IdentityIdAsync(Guid.Parse(request.IdentityId), Guid.Parse(request.ResourceId));
            
            return AccessServiceMapper.ToGrpcAccessResponce(access);

        }

        public override async Task<AccessResponce> UpdateAccess(AccessDataRequest request, ServerCallContext context)
        {
            Access access = await _accessRepository.GetByResourceId_IdentityIdAsync(Guid.Parse(request.IdentityId), Guid.Parse(request.ResourceId));
            access.AccessData = request.AccessData;
            await _accessRepository.UpdateAsync(access);

            return AccessServiceMapper.ToGrpcAccessResponce(access);

        }

        public override async Task<EmptyAccessResponce> DeleteAccess(GetAccessByIdsRequest request, ServerCallContext context)
        {
            Access access = await _accessRepository.GetByResourceId_IdentityIdAsync(Guid.Parse(request.IdentityId), Guid.Parse(request.ResourceId));
            await _accessRepository.DeleteAsync(access.Id);

            return new EmptyAccessResponce();

        }

    }

    public static class AccessServiceMapper
    {
        public static AccessResponce ToGrpcAccessResponce(Access access)
        {
            AccessResponce responce = new AccessResponce();
            responce.Id = access.Id.ToString();
            responce.IdentityId = access.IdentityId.ToString();
            responce.ResourceId = access.ResourceId.ToString();
            responce.AccessData = access.AccessData;
            responce.HasAccess = access.AccessData != "Denied";
            return responce;
        }
    }
}
