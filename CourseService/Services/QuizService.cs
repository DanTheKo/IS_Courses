
using Grpc.Core;
using CourseService.Grpc;

using CourseService.Repositories.Quizzes;
using CourseService.Data;

namespace CourseService.Services
{

    public class QuizService : QuizCrudService.QuizCrudServiceBase
    {
        private readonly QuizRepository _repository;

        public QuizService(CourseDbContext dbContext)
        {
            _repository = new QuizRepository(dbContext);
        }

        public override async Task<EntityResponse> Create(CreateRequest request, ServerCallContext context)
        {
            try
            {
                request.Quiz.Id = "";
                var entity = ToModel(request.Quiz);
                await _repository.AddAsync(entity);
                return new EntityResponse() { Quiz = ToProto(entity) };
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
                var entity = await _repository.GetWithChildrenAsync(id);
                return new EntityResponse() { Quiz = ToProto(entity) };
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
                var entity = request.Quiz;
                await _repository.UpdateAsync(ToModel(entity));
                return new EntityResponse() { Quiz = entity};
            }
            catch (Exception ex)
            {
                return new EntityResponse();
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
                    response.Entities.Add(new EntityResponse() {Quiz = ToProto(entity) });
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private Quiz ToProto(Models.Quizzes.Quiz quiz)
        {
            if (quiz == null) return null;
            Quiz protoQuiz = new Quiz
            {
                Id = quiz.Id.ToString(),
                CourseItemId = quiz.CourseItemId.ToString(),
                Title = quiz.Title,
                Description = quiz.Description,
                Type = quiz.Type,
            };
            protoQuiz.QuestionsIds.Add(quiz.Questions.Select(e => e.Id.ToString()));
            protoQuiz.QuizResponsesIds.Add(quiz.QuizResponses.Select(e => e.Id.ToString()));
            return protoQuiz;
        }

        private Models.Quizzes.Quiz ToModel(Quiz quiz)
        {
            if (quiz == null) return null;
            Models.Quizzes.Quiz modelQuiz = new(quiz.Id, quiz.CourseItemId, quiz.Title, quiz.Type, quiz.Description);
            return modelQuiz;
        }
    }
}
