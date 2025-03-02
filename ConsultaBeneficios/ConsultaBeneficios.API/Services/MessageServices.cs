using ConsultaBeneficios.API.Interfaces;
using RabbitMQ.Client;

namespace ConsultaBeneficios.API.Services
{
    public class MessageServices : IMessageServices
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly ILogger _logger;

        public MessageServices(IConfiguration configuration, ILogger<MessageServices> logger)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = configuration.GetValue<string>("RabbitMQConnection:Host"),
                UserName = configuration.GetValue<string>("RabbitMQConnection:Username"),
                Password = configuration.GetValue<string>("RabbitMQConnection:Password")
            };
            _logger = logger;
        }

        public async void Publish(string queue, byte[] message)
        {
            _logger.LogInformation($"Publicando uma nova mensagem para a fila {queue}");

            using var conexao = await _connectionFactory.CreateConnectionAsync();
            using var canal = await conexao.CreateChannelAsync();

            await canal.QueueDeclareAsync(queue, false, false, false);

            await canal.BasicPublishAsync("", queue, message);
        }
    }
}
