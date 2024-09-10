using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Factories.WebApi.BLL.Services
{
    public class TankUpdateService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IModel? _channel;
        private IConnection? _connection;
        private readonly string _queueName = "tanks_worker_updates";

        public TankUpdateService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory() { DispatchConsumersAsync = true, HostName = "rabbitmq", Port = 5672 }; // TODO: добавить конфигурацию хоста
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await ProcessMessage(message);

                _channel!.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

            await Task.CompletedTask;
        }

        private async Task ProcessMessage(string message)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var tanksRepository = scope.ServiceProvider.GetRequiredService<IRepository<Tank>>();
            var updateInfo = JsonSerializer.Deserialize<TankUpdateInfo>(message);

            var tank = tanksRepository.Get(updateInfo!.TankId);
            if (tank != null)
            {
                tank.Volume = updateInfo.UpdatedVolume;
                await tanksRepository.UpdateAsync(tank.Id, tank);
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }

    public class TankUpdateInfo
    {
        public int TankId { get; set; }
        public int UpdatedVolume { get; set; }
    }
}