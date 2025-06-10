using GatewayAPI.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace GatewayAPI.RabbitMq
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private IConnection _connection;
        private IChannel _channel;
        private ILogger<RabbitMqConsumerService> _logger;
        private ConnectionFactory _factory;

        private string Queue = string.Empty;

        public RabbitMqConsumerService( string hostName = "localhost", int Port = 5672, string queue = "BaseQueue", ILogger<RabbitMqConsumerService> logger = null)
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
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            if (_channel == null || _channel.IsClosed)
                await InitializeAsync();

            try
            {
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (ch, ea) =>
                {
                    var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                    // Каким-то образом обрабатываем полученное сообщение
                    Console.WriteLine($"Получено сообщение: {content}");

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                };

                await _channel.BasicConsumeAsync(Queue, false, consumer);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to consume message RabbitMQ");
                throw;
            }


            return;
        }

        public override async void Dispose()
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
            base.Dispose();
        }
    }
}
