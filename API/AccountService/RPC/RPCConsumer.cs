using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace AccountService.RPC
{
    public class RPCConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;

        public RPCConsumer()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("user", durable: false, exclusive: false, autoDelete: false, arguments: null);
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

                Console.WriteLine("Product message received: {0}", json);

                // Delete the message
                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };
            //read the message
            _channel.BasicConsume(queue: "user", false, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
