
using ConsultaBeneficios.API.Interfaces;
using ConsultaBeneficios.API.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ConsultaBeneficios.API.Consumers
{
    public class BeneficiarioConsumer : BackgroundService
    {
        private readonly IBeneficiarioServices _beneficiarioServices;
        private readonly ILogger<BeneficiarioConsumer> _logger;
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IChannel _channel;
        private const string QUEUE_NAME = "cpf-beneficiarios";

        public BeneficiarioConsumer(IBeneficiarioServices beneficiarioServices, IConfiguration configuration, ILogger<BeneficiarioConsumer> logger)
        {
            _beneficiarioServices = beneficiarioServices;
            _logger = logger;

            _factory = new ConnectionFactory
            {
                HostName = configuration.GetValue<string>("RabbitMQConnection:Host"),
                UserName = configuration.GetValue<string>("RabbitMQConnection:Username"),
                Password = configuration.GetValue<string>("RabbitMQConnection:Password")
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Iniciando o consumidor {QUEUE_NAME}");

            int retryCount = 10;

            while (!stoppingToken.IsCancellationRequested && retryCount > 0)
            {
                try
                {
                    _connection = await _factory.CreateConnectionAsync();
                    _channel = await _connection.CreateChannelAsync();

                    await _channel.QueueDeclareAsync(QUEUE_NAME, false, false, false);

                    var consumer = new AsyncEventingBasicConsumer(_channel);
                    consumer.ReceivedAsync += Consumer_ReceivedAsync;

                    await _channel.BasicConsumeAsync(QUEUE_NAME, false, consumer);

                    _logger.LogInformation("Conexão com o RabbitMQ estabelecida.");

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao conectar ao RabbitMQ: {ex.Message}. Tentando novamente...");
                    await Task.Delay(10000, stoppingToken);
                    retryCount--;
                }
            }

            if (retryCount == 0)
            {
                _logger.LogError("Falha ao conectar ao RabbitMQ após várias tentativas.");
            }
        }

        private async Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs @event)
        {
            var textBytes = @event.Body.ToArray();
            var cpf = Encoding.UTF8.GetString(textBytes);

            _logger.LogInformation($"Recebendo uma nova mensagem para a fila {QUEUE_NAME}, CPF: {cpf}");

            await _beneficiarioServices.ConsultarBeneficiarioAsync(cpf);

            await _channel.BasicAckAsync(@event.DeliveryTag, false);
        }
    }
}
