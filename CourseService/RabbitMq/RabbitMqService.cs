using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CourseService.RabbitMq
{
    public class RabbitMqService : IRabbitMqService, IAsyncDisposable
    {
        private IConnection _connection;
        private IChannel _channel;
        private ILogger<RabbitMqService> _logger;
        private ConnectionFactory _factory;

        private string Queue;

        public RabbitMqService(string hostName = "localhost", int Port = 5672, string queue = "BaseQueue", ILogger<RabbitMqService> logger = null)
        {
            Queue = queue;
            _factory = new ConnectionFactory() { HostName = hostName, Port = Port, UserName = "guest", Password = "guest" };
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _connection = await _factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync(
                    queue: Queue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to initialize RabbitMQ connection");
                throw;
            }
        }

        public async Task SendMessageAsync(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            await SendMessageAsync(message);
        }

        public async Task SendMessageAsync(string message)
        {
            if (_channel == null || _channel.IsClosed)
                await InitializeAsync();

            try
            {
                var body = Encoding.UTF8.GetBytes(message);

                await _channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: Queue,
                    body: body);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to send message to RabbitMQ");
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null && _channel.IsOpen)
            {
                await _channel.CloseAsync();
                _channel.Dispose();
            }

            if (_connection != null && _connection.IsOpen)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }
        }
    }

}