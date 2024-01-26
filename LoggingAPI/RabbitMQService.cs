using LoggingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace LoggingAPI
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

        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "soa_rv1_upp3", routingKey: "rv1_upp3", basicProperties: null, body: body);
        }

        public void SendLog(LoggingEntry logEntry)
        {
            var json = JsonSerializer.Serialize(logEntry);
            var body = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(exchange: "soa_rv1_upp3", routingKey: "rv1_upp3", basicProperties: null, body: body);
        }

        public List<LoggingEntry> GetLogs(DateTime startDate, DateTime endDate)
        {
            var logs = new List<LoggingEntry>();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var logEntry = JsonSerializer.Deserialize<LoggingEntry>(message);

                if (logEntry != null && logEntry.Timestamp >= startDate && logEntry.Timestamp <= endDate)
                {
                    logs.Add(logEntry);
                }

                // Optionally acknowledge the message or re-queue it
                // _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: "soa_rv1_upp3", autoAck: true, consumer: consumer);

            return logs;
        }

        public void ClearQueue(string queueName)
        {
            _channel.QueuePurge(queueName);
        }

        public List<LoggingEntry> GetAllMessages()
        {
            var messages = new List<LoggingEntry>();

            while (true)
            {
                var result = _channel.BasicGet("your_queue_name", true);
                if (result == null) break;

                var message = Encoding.UTF8.GetString(result.Body.ToArray());
                var logEntry = JsonSerializer.Deserialize<LoggingEntry>(message);
                if (logEntry != null)
                {
                    messages.Add(logEntry);
                }
            }

            return messages;
        }

    }
}
