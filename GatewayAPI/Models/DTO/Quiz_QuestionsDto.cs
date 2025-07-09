using GatewayAPI.Grpc;

namespace GatewayAPI.Models.DTO
{
    public class Quiz_QuestionsDto
    {
        public Quiz Quiz { get; set; }
        public List<Question> Questions { get; set; }
    }
}
