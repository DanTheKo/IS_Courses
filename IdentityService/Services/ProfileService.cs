
using Grpc.Core;
using IdentityService;
using IdentityService.Data;
using IdentityService.Grpc;
using IdentityService.Repositories;

namespace IdentityService.Services
{

    public class ProfileService : ProfileGrpcService.ProfileGrpcServiceBase
    {
        private readonly ProfileRepository _repository;

        public ProfileService(IdentityDbContext dbContext)
        {
            _repository = new ProfileRepository(dbContext);
        }

        public override async Task<EntityResponse> Create(CreateRequest request, ServerCallContext context)
        {
            try
            {
                request.Profile.Id = "";
                var entity = ToModel(request.Profile);
                await _repository.AddAsync(entity);
                return new EntityResponse() { Profile = ToProto(entity) };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<EntityResponse> Read(ReadRequest request, ServerCallContext context)
        {
            try
            {
                var id = Guid.Parse(request.Id);
                var entity = await _repository.GetByIdAsync(id);
                return new EntityResponse() { Profile = ToProto(entity) };
            }
            catch (Exception ex)
            {
                return new EntityResponse();
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));

            }
        }

        public override async Task<EntityResponse> ReadByIdentityId(ReadRequest request, ServerCallContext context)
        {
            try
            {
                var id = Guid.Parse(request.Id);
                var entity = await _repository.GetByIdentityAsync(id.ToString());
                return new EntityResponse() { Profile = ToProto(entity) };
            }
            catch (Exception ex)
            {
                return new EntityResponse();
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));

            }
        }


        public override async Task<EntityResponse> Update(UpdateRequest request, ServerCallContext context)
        {
            try
            {
                var entity = request.Profile;
                await _repository.UpdateAsync(ToModel(entity));
                return new EntityResponse() { Profile = entity };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }


        public override async Task<Google.Protobuf.WellKnownTypes.Empty> Delete(DeleteRequest request, ServerCallContext context)
        {

            try
            {
                var id = Guid.Parse(request.Id);
                await _repository.DeleteAsync(id);
                return new Google.Protobuf.WellKnownTypes.Empty();
            }
            catch (Exception ex)
            {
                return new Google.Protobuf.WellKnownTypes.Empty();
            }
        }

        public override async Task<ListResponse> List(ListRequest request, ServerCallContext context)
        {

            try
            {
                var entities = await _repository.GetAllAsync();
                var response = new ListResponse();

                foreach (var entity in entities)
                {
                    response.Entities.Add(new EntityResponse() { Profile = ToProto(entity) });
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private Profile ToProto(Models.Profile profile)
        {
            if (profile == null) return null;
            Profile protoProfile = new Profile()
            {
                Id = profile.Id.ToString(),
                IdentityId = profile.IdentityId.ToString(),
                Firstname = profile.FirstName,
                Lastname = profile.LastName,
                Middlename = profile.MiddleName,
                Status = profile.Status,
                ProfileImageUrl = profile.ProfileImageUrl

            };
            return protoProfile;
        }

        private Models.Profile ToModel(Profile profile)
        {
            if (profile == null) return null;
            Models.Profile modelProfile = new(profile.Id, profile.IdentityId, profile.Firstname, profile.Lastname, profile.Middlename, profile.Status, profile.ProfileImageUrl);
            return modelProfile;
        }
    }
}
