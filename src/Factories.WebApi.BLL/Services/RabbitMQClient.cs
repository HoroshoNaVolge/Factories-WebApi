using RabbitMQ.Client;
using System.Text;

namespace Factories.WebApi.BLL.Services
{
    public class RabbitMQClient
    {
        private readonly IModel _channel;

        public RabbitMQClient(string connectionString)
        {
            var factory = new ConnectionFactory() { HostName=connectionString};
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "tanks_worker_updates", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "tanks_worker_updates", basicProperties: null, body: body);
        }
    }
}
