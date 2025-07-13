using GatewayAPI.Grpc;

namespace GatewayAPI.Models.DTO
{
    public class QuizResultDto
    {

        public List<Question> Questions { get; set; }
        public List<QuestionAnswer> QuestionAnswers { get; set; }
        public List<QuestionResultDto> Results { get; set; }
        public int TotalScore { get; set; }
        public int MaxPossibleScore { get; set; }
        public double Percentage => MaxPossibleScore > 0
            ? Math.Round((double)TotalScore / MaxPossibleScore * 100, 2)
            : 0;


    }

    public class QuestionResultDto
    {
        public string QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string CorrectAnswer { get; set; }
        public string UserAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public int Score { get; set; }
        public int MaxScore { get; set; }
    }
}
