using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaProducer
{
    public class KafkaProducerService : IHostedService
    {
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerService(ILogger<KafkaProducerService> logger)
        {
            _logger = logger;
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            for (var i = 0; i < 5; i++)
            {
                var value = $"Event N {i}";
                _logger.LogInformation($"Sending >> {value}");
                await _producer.ProduceAsync(
                    "demo-topic",
                    new Message<Null, string> { Value = value },
                    cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _producer?.Dispose();
            _logger.LogInformation($"{nameof(KafkaProducerService)} stopped");
            return Task.CompletedTask;
        }
    }
}
