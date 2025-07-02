
using Grpc.Core;
using CourseService.Grpc;

using CourseService.Repositories.Quizzes;
using CourseService.Data;

namespace CourseService.Services
{

    public class QuestionService : QuestionCrudService.QuestionCrudServiceBase
    {
        private readonly QuestionRepository _repository;

        public QuestionService(CourseDbContext dbContext)
        {
            _repository = new QuestionRepository(dbContext);
        }

        public override async Task<EntityResponse> Create(CreateRequest request, ServerCallContext context)
        {
            try
            {
                var entity = ToModel(request.Question);
                await _repository.AddAsync(entity);
                return new EntityResponse() { Question = ToProto(entity) };
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
                return new EntityResponse() { Question = ToProto(entity) };
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
                var entity = request.Question;
                await _repository.UpdateAsync(ToModel(entity));
                return new EntityResponse() { Question = entity };
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
                    response.Entities.Add(new EntityResponse() { Question = ToProto(entity) });
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private Question ToProto(Models.Quizzes.Question question)
        {
            Question protoQuestion = new Question()
            {
                Id = question.Id.ToString(),
                QuizId = question.QuizId.ToString(),
                QuestionText = question.QuestionText,
                QuestionType = question.QuestionType,
                Options = question.Options,
                CorrectAnswer = question.CorrectAnswer,
                MaxScore = question.MaxScore,
                Order = question.Order
            };
            protoQuestion.QuestionAnswersIds.Add(question.QuestionAnswers.Select(e => e.Id.ToString()));
            return protoQuestion;
        }

        private Models.Quizzes.Question ToModel(Question question)
        {
            Models.Quizzes.Question modelQuestion = new(question.Id, question.QuizId, question.QuestionType, question.QuestionText, question.Options, question.CorrectAnswer, question.MaxScore, question.Order);
            return modelQuestion;
        }
    }
}
