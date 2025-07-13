using GatewayAPI.Grpc;

namespace GatewayAPI.Models.DTO
{
    public class QuizResponse_QuestionAnswersDto
    {
        public QuizResponse QuizResponse { get; set; }
        public List<QuestionAnswer> QuestionAnswers { get; set; }
        public List<Question> Questions { get; set; }
    }
}
