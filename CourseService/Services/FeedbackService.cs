
using Grpc.Core;
using CourseService.Grpc;

using CourseService.Repositories.Quizzes;
using CourseService.Data;

namespace CourseService.Services
{

    public class FeedbackService : FeedbackCrudService.FeedbackCrudServiceBase
    {
        private readonly FeedbackRepository _repository;

        public FeedbackService(CourseDbContext dbContext)
        {
            _repository = new FeedbackRepository(dbContext);
        }

        public override async Task<EntityResponse> Create(CreateRequest request, ServerCallContext context)
        {
            try
            {
                var entity = ToModel(request.Feedback);
                await _repository.AddAsync(entity);
                return new EntityResponse() { Feedback = ToProto(entity) };
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
                return new EntityResponse() { Feedback = ToProto(entity) };
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
                var entity = request.Feedback;
                await _repository.UpdateAsync(ToModel(entity));
                return new EntityResponse() { Feedback = entity };
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
                    response.Entities.Add(new EntityResponse() { Feedback = ToProto(entity) });
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private Feedback ToProto(Models.Quizzes.Feedback feedback)
        {
            Feedback protoFeedback = new Feedback()
            {
                Id = feedback.Id.ToString(),
                Comment = feedback.Comment,
                ExaminerId = feedback.ExaminerId.ToString(), 
                QuestionAnswerId = feedback.QuestionAnswerId.ToString(),
                Rating = feedback.Rating
            };
            return protoFeedback;
        }

        private Models.Quizzes.Feedback ToModel(Feedback feedback)
        {
            Models.Quizzes.Feedback modelFeedback = new(feedback.Id, feedback.QuestionAnswerId, feedback.ExaminerId, feedback.Comment, feedback.Rating);
            return modelFeedback;
        }
    }
}
