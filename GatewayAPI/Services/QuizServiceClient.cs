
using AutoMapper;
using GatewayAPI.Grpc;
using global::Grpc.Core;
using global::Grpc.Net.Client;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GatewayAPI.Services
{
    public class QuizServiceClient : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly QuizCrudService.QuizCrudServiceClient _quizClient;
        private readonly QuizResponseCrudService.QuizResponseCrudServiceClient _quizResponseClient;
        private readonly QuestionCrudService.QuestionCrudServiceClient _questionClient;
        private readonly QuestionAnswerCrudService.QuestionAnswerCrudServiceClient _questionAnswerClient;
        private readonly FeedbackCrudService.FeedbackCrudServiceClient _feedbackClient;

        public QuizServiceClient(string serverUrl)
        {
            _channel = GrpcChannel.ForAddress(serverUrl);
            _quizClient = new QuizCrudService.QuizCrudServiceClient(_channel);
            _quizResponseClient = new QuizResponseCrudService.QuizResponseCrudServiceClient(_channel);
            _questionClient = new QuestionCrudService.QuestionCrudServiceClient(_channel);
            _questionAnswerClient = new QuestionAnswerCrudService.QuestionAnswerCrudServiceClient(_channel);
            _feedbackClient = new FeedbackCrudService.FeedbackCrudServiceClient(_channel);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            GC.SuppressFinalize(this);
        }

/*        private async Task<TResponse> CallServiceAsync<TResponse>(Func<QuizCrudService.QuizCrudServiceClient, Task<TResponse>> action)
        {
            try
            {
                return await action(_client);
            }
            catch (RpcException)
            {
                throw;
            }
        }*/
        #region Feedback
        public async Task<Feedback> CreateFeedbackAsync(Feedback feedback)
        {
            var request = new CreateRequest
            {
                Feedback = feedback
            };

            var response = await _feedbackClient.CreateAsync(request);
            return response.Feedback;
        }

        public async Task<Feedback> UpdateFeedbackAsync(Feedback feedback)
        {
            var request = new UpdateRequest
            {
                Feedback = feedback
            };

            var response = await _feedbackClient.UpdateAsync(request);
            return response.Feedback;
        }

        public async Task<Feedback> GetFeedbackAsync(string id)
        {
            var request = new ReadRequest
            {
                EntityType = "feedback",
                Id = id
            };

            var response = await _feedbackClient.ReadAsync(request);
            return response.Feedback;
        }


        public async Task DeleteFeedbackAsync(string id)
        {
            var request = new DeleteRequest
            {
                EntityType = "feedback",
                Id = id
            };

            await _feedbackClient.DeleteAsync(request);
        }

        public async Task<IEnumerable<Feedback>> GetAllFeedbacksAsync()
        {
            var request = new ListRequest
            {
                EntityType = "feedback"
            };

            var response = await _feedbackClient.ListAsync(request);
            return response.Entities.Select(e => e.Feedback);
        }
        #endregion

        #region Questions
        public async Task<Question> CreateQuestionAsync(Question question)
        {
            var request = new CreateRequest
            {
                Question = question
            };

            var response = await _questionClient.CreateAsync(request);
            return response.Question;
        }

        public async Task<Question> UpdateQuestionAsync(Question question)
        {
            var request = new UpdateRequest
            {
                Question = question
            };

            var response = await _questionClient.UpdateAsync(request);
            return response.Question;
        }
        public async Task<Question> GetQuestionAsync(string id)
        {
            var request = new ReadRequest
            {
                EntityType = "question",
                Id = id
            };

            var response = await _questionClient.ReadAsync(request);
            return response.Question;
        }

        public async Task DeleteQuestionAsync(string id)
        {
            var request = new DeleteRequest
            {
                EntityType = "question",
                Id = id
            };

            await _questionClient.DeleteAsync(request);
        }
        #endregion

        #region Question Answer
        public async Task<QuestionAnswer> CreateQuestionAnswerAsync(QuestionAnswer questionAnswer)
        {
            var request = new CreateRequest
            {
                QuestionAnswer = questionAnswer
            };

            var response = await _questionAnswerClient.CreateAsync(request);
            return response.QuestionAnswer;
        }


        public async Task<QuestionAnswer> UpdateQuestionAnswerAsync(QuestionAnswer questionAnswer)
        {
            var request = new UpdateRequest
            {
                QuestionAnswer = questionAnswer
            };

            var response = await _questionAnswerClient.UpdateAsync(request);
            return response.QuestionAnswer;
        }

        public async Task<QuestionAnswer> GetQuestionAnswerAsync(string id)
        {
            var request = new ReadRequest
            {
                EntityType = "questionanswer",
                Id = id
            };

            var response = await _questionAnswerClient.ReadAsync(request);
            return response.QuestionAnswer;
        }

        public async Task DeleteQuestionAnswerAsync(string id)
        {
            var request = new DeleteRequest
            {
                EntityType = "questionanswer",
                Id = id
            };

            await _questionAnswerClient.DeleteAsync(request);
        }

        public async Task<IEnumerable<QuestionAnswer>> GetAllQuestionAnswersAsync()
        {
            var request = new ListRequest
            {
                EntityType = "questionanswer"
            };

            var response = await _questionAnswerClient.ListAsync(request);
            return response.Entities.Select(e => e.QuestionAnswer);
        }
        #endregion

        #region Quizzes
        public async Task<Quiz> CreateQuizAsync(Quiz quiz)
        {
            var request = new CreateRequest
            {
                Quiz = quiz
            };

            var response = await _quizClient.CreateAsync(request);
            return response.Quiz;
        }

        public async Task<Quiz> UpdateQuizAsync(Quiz quiz)
        {
            var request = new UpdateRequest
            {
                Quiz = quiz
            };

            var response = await _quizClient.UpdateAsync(request);
            return response.Quiz;
        }

        public async Task<Quiz> GetQuizAsync(string id)
        {
            var request = new ReadRequest
            {
                EntityType = "quiz",
                Id = id
            };

            var response = await _quizClient.ReadAsync(request);
            return response.Quiz;
        }

        public async Task DeleteQuizAsync(string id)
        {
            var request = new DeleteRequest
            {
                EntityType = "quiz",
                Id = id
            };

            await _quizClient.DeleteAsync(request);
        }

        public async Task<IEnumerable<Quiz>> GetAllQuizzesAsync()
        {
            var request = new ListRequest
            {
                EntityType = "quiz"
            };

            var response = await _quizClient.ListAsync(request);
            return response.Entities.Select(e => e.Quiz);
        }
        #endregion

        #region Quizzes Responses
        public async Task<QuizResponse> CreateQuizResponseAsync(QuizResponse quizResponse)
        {
            var request = new CreateRequest
            {
                QuizResponse = quizResponse
            };

            var response = await _quizResponseClient.CreateAsync(request);
            return response.QuizResponse;
        }

        public async Task<QuizResponse> UpdateQuizResponseAsync(QuizResponse quizResponse)
        {
            var request = new UpdateRequest
            {
                QuizResponse = quizResponse
            };

            var response = await _quizResponseClient.UpdateAsync(request);
            return response.QuizResponse;
        }

        public async Task<QuizResponse> GetQuizResponseAsync(string id)
        {
            var request = new ReadRequest
            {
                EntityType = "quizresponse",
                Id = id
            };

            var response = await _quizResponseClient.ReadAsync(request);
            return response.QuizResponse;
        }

        public async Task DeleteQuizResponseAsync(string id)
        {
            var request = new DeleteRequest
            {
                EntityType = "quizresponse",
                Id = id
            };

            await _quizResponseClient.DeleteAsync(request);
        }

        public async Task<IEnumerable<QuizResponse>> GetAllQuizResponceAsync()
        {
            var request = new ListRequest
            {
                EntityType = "quizresponse"
            };

            var response = await _quizResponseClient.ListAsync(request);
            return response.Entities.Select(e => e.QuizResponse);
        }
        #endregion

        
    }
}
