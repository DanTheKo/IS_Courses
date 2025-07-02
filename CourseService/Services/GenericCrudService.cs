using FluentNHibernate.Automapping;
using Grpc.Core;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using global::CourseService.Grpc;
using global::CourseService.Repositories.Quizzes;
using Google.Protobuf;
using CSharpFunctionalExtensions;

namespace CourseService.Services
{

    public class GenericCrudService<TEntity, TId, TProtoEntity>
        where TEntity : Entity<Guid>
        where TProtoEntity : class, new()
    {
        private readonly IEntityRepository<TEntity, TId> _repository;
        private readonly IMapper _mapper;
        private readonly string _entityTypeName;



        public GenericCrudService(
            IEntityRepository<TEntity, TId> repository,
            IMapper mapper, string serviceName)
        {
            _repository = repository;
            _mapper = mapper;
            _entityTypeName = typeof(TEntity).Name.ToLower();
        }


        private TEntity GetEntityFromRequest(IMessage request)
        {
            var fieldDescriptor = request.Descriptor.FindFieldByName(_entityTypeName);
            if (fieldDescriptor == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Request does not contain {_entityTypeName} entity"));
            }

            var protoEntity = (TProtoEntity)fieldDescriptor.Accessor.GetValue(request);
            return _mapper.Map<TEntity>(protoEntity);
        }


        private EntityResponse CreateEntityResponse(TEntity entity)
        {
            var response = new EntityResponse();
            var protoEntity = _mapper.Map<TProtoEntity>(entity);

            // Используем reflection для установки соответствующего поля
            var property = typeof(EntityResponse)
                .GetProperties()
                .FirstOrDefault(p => p.Name.Equals(_entityTypeName, StringComparison.OrdinalIgnoreCase));

            if (property != null)
            {
                property.SetValue(response, protoEntity);
            }
            else
            {
                throw new InvalidOperationException($"No matching property found in EntityResponse for {_entityTypeName}");
            }

            return response;
        }

        public async Task<EntityResponse> Create(CreateRequest request, ServerCallContext context)
        {
            try
            {
                var entity = GetEntityFromRequest(request);
                await _repository.AddAsync(entity);
                return CreateEntityResponse(entity);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        // Аналогично исправляем остальные методы (Read, Update, List)
        // Заменяем все случаи использования [] на CreateEntityResponse


        public async Task<EntityResponse> Read(ReadRequest request, ServerCallContext context)
        {
            if (request.EntityType != _entityTypeName)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Invalid entity type. Expected: {_entityTypeName}"));
            }

            try
            {
                var id = ParseId(request.Id);
                var entity = await _repository.GetByIdAsync(id);
                return CreateEntityResponse(entity);
            }
            catch (EntityNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
        }


        public async Task<EntityResponse> Update(UpdateRequest request, ServerCallContext context)
        {
            try
            {
                var entity = GetEntityFromRequest(request);
                await _repository.UpdateAsync(entity);
                return CreateEntityResponse(entity);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }


        public async Task<Google.Protobuf.WellKnownTypes.Empty> Delete(DeleteRequest request, ServerCallContext context)
        {
            if (request.EntityType != _entityTypeName)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Invalid entity type. Expected: {_entityTypeName}"));
            }

            try
            {
                var id = ParseId(request.Id);
                await _repository.DeleteAsync(id);
                return new Google.Protobuf.WellKnownTypes.Empty();
            }
            catch (EntityNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
        }


        public async Task<ListResponse> List(ListRequest request, ServerCallContext context)
        {
            if (request.EntityType != _entityTypeName)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Invalid entity type. Expected: {_entityTypeName}"));
            }

            try
            {
                var entities = await _repository.GetAllAsync();
                var response = new ListResponse();

                foreach (var entity in entities)
                {
                    response.Entities.Add(CreateEntityResponse(entity));
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }


        private TId ParseId(string id)
        {
            if (typeof(TId) == typeof(Guid))
            {
                return (TId)(object)Guid.Parse(id);
            }
            return (TId)Convert.ChangeType(id, typeof(TId));
        }
    }
}
