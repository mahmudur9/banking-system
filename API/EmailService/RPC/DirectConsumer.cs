using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AccountService.RPC
{
    public class DirectConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;

        private const string ExchangeName = "RandomDirectExchangeName";
        private const string EmailQueueName = "EmailQueue";

        public DirectConsumer()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);

            _channel.QueueDeclare(EmailQueueName, false, false, false, null);
            _channel.QueueBind(EmailQueueName, ExchangeName, routingKey: "EmailRoute");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, eventArgs) => {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Convert string to json
                dynamic json = JsonConvert.DeserializeObject(message);

                Console.WriteLine("Email content received: {0}", json);

                // Delete the message
                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };
            //read the message
            _channel.BasicConsume(queue: EmailQueueName, false, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
