
using Grpc.Core;
using CourseService.Grpc;

using CourseService.Repositories.Quizzes;
using CourseService.Data;

namespace CourseService.Services
{

    public class QuestionAnswerService : QuestionAnswerCrudService.QuestionAnswerCrudServiceBase
    {
        private readonly QuestionAnswerRepository _repository;

        public QuestionAnswerService(CourseDbContext dbContext)
        {
            _repository = new QuestionAnswerRepository(dbContext);
        }

        public override async Task<EntityResponse> Create(CreateRequest request, ServerCallContext context)
        {
            try
            {
                var entity = ToModel(request.QuestionAnswer);
                await _repository.AddAsync(entity);
                return new EntityResponse() { QuestionAnswer = ToProto(entity) };
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
                return new EntityResponse() { QuestionAnswer = ToProto(entity) };
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
                var entity = request.QuestionAnswer;
                await _repository.UpdateAsync(ToModel(entity));
                return new EntityResponse() { QuestionAnswer = entity };
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
                    response.Entities.Add(new EntityResponse() { QuestionAnswer = ToProto(entity) });
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private QuestionAnswer ToProto(Models.Quizzes.QuestionAnswer questionAnswer)
        {
            QuestionAnswer protoQuestionAnswer = new QuestionAnswer()
            {
                Id = questionAnswer.Id.ToString(),
                QuestionId = questionAnswer.QuestionId.ToString(),
                QuizResponseId = questionAnswer.QuizResponceId.ToString(),
                AnswerText = questionAnswer.AnswerText,
                SelectedOptions = questionAnswer.SelectedOptions,
            };
            protoQuestionAnswer.FeedbacksIds.Add(questionAnswer.Feedbacks.Select(e => e.Id.ToString()));
            return protoQuestionAnswer;
        }

        private Models.Quizzes.QuestionAnswer ToModel(QuestionAnswer questionAnswer)
        {
            Models.Quizzes.QuestionAnswer modelQuestionAnswer = new(questionAnswer.Id, questionAnswer.QuestionId, questionAnswer.QuizResponseId, questionAnswer.AnswerText, questionAnswer.SelectedOptions);
            return modelQuestionAnswer;
        }
    }
}
