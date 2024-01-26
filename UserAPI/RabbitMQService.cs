using UserAPI;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace UserAPI
{
    public class RabbitMQService
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQService()
        {
            _factory = new ConnectionFactory()
            {
                HostName = "studentdocker.informatika.uni-mb.si",
                Port = 5672,
                UserName = "student",
                Password = "student123"
            };

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            //_channel.ExchangeDeclare(exchange: "soa_rv1_upp3", type: "direct", durable: true);
            _channel.QueueDeclare(queue: "soa_rv1_upp3", durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: "soa_rv1_upp3", exchange: "soa_rv1_upp3", routingKey: "rv1_upp3");
        }

        public void SendLog(LoggingEntry logEntry)
        {
            var json = JsonSerializer.Serialize(logEntry);
            var body = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(exchange: "soa_rv1_upp3", routingKey: "rv1_upp3", basicProperties: null, body: body);
        }

    }
}
