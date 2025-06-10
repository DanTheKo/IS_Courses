namespace CourseService.RabbitMq
{
    public interface IRabbitMqService : IAsyncDisposable
    {
        Task SendMessageAsync(object obj);
        Task SendMessageAsync(string message);
    }
}
