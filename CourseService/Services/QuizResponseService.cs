
using Grpc.Core;
using CourseService.Grpc;

using CourseService.Repositories.Quizzes;
using CourseService.Data;

namespace CourseService.Services
{

    public class QuizResponseService : QuizResponseCrudService.QuizResponseCrudServiceBase
    {
        private readonly QuizResponseRepository _repository;

        public QuizResponseService(CourseDbContext dbContext)
        {
            _repository = new QuizResponseRepository(dbContext);
        }

        public override async Task<EntityResponse> Create(CreateRequest request, ServerCallContext context)
        {
            try
            {
                var entity = ToModel(request.QuizResponse);
                await _repository.AddAsync(entity);
                return new EntityResponse() { QuizResponse = ToProto(entity) };
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
                return new EntityResponse() { QuizResponse = ToProto(entity) };
            }
            catch (EntityNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
        }


        public override async Task<EntityResponse> Update(UpdateRequest request, ServerCallContext context)
        {
            try
            {
                var entity = request.QuizResponse;
                await _repository.UpdateAsync(ToModel(entity));
                return new EntityResponse() { QuizResponse = entity};
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
            catch (EntityNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
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
                    response.Entities.Add(new EntityResponse() {QuizResponse = ToProto(entity) });
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private QuizResponse ToProto(Models.Quizzes.QuizResponse quizResponse)
        {
            QuizResponse protoQuizResponse = new QuizResponse();
            protoQuizResponse.Id = quizResponse.Id.ToString();
            protoQuizResponse.IdentityId = quizResponse.IdentityId.ToString();
            protoQuizResponse.QuizId = quizResponse.QuizId.ToString();
            protoQuizResponse.QuestionAnswersIds.Add(quizResponse.QuestionAnswers.Select(e => e.Id.ToString()));
            return protoQuizResponse;
        }

        private Models.Quizzes.QuizResponse ToModel(QuizResponse quizResponse)
        {
            Models.Quizzes.QuizResponse modelQuizResponse = new(quizResponse.Id, quizResponse.QuizId, quizResponse.IdentityId);
            return modelQuizResponse;
        }
    }
}
