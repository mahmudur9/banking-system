using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.RPC
{
    public class DirectProducer : IDirectProducer
    {
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private IConnection _connection;

        private const string ExchangeName = "RandomDirectExchangeName";
        private const string AccountQueueName = "AccountQueue";
        private const string EmailQueueName = "EmailQueue";

        public DirectProducer()
        {
            _hostname = "localhost";
            _username = "guest";
            _password = "guest";
        }

        public void SendProductMessage<T>(T message)
        {
            if (ConnectionExists())
            {
                //Here we create channel with session and model
                using var channel = _connection.CreateModel();
                //declare the queue after mentioning name and a few property related to that
                channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: false);

                channel.QueueDeclare(AccountQueueName, false, false, false, null);
                channel.QueueDeclare(EmailQueueName, false, false, false, null);

                channel.QueueBind(AccountQueueName, ExchangeName, routingKey: "AccountRoute");
                channel.QueueBind(EmailQueueName, ExchangeName, routingKey: "EmailRoute");
                //Serialize the message
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);
                //put the data on to the product queue
                channel.BasicPublish(exchange: ExchangeName, "AccountRoute", basicProperties: null, body: body);
                channel.BasicPublish(exchange: ExchangeName, "EmailRoute", basicProperties: null, body: body);
            }
        }

        private void CreateConnection()
        {
            try
            {
                //Here we specify the Rabbit MQ Server
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };
                //Create the RabbitMQ connection using connection factory details as i mentioned above
                _connection = factory.CreateConnection();
            }
            catch (Exception)
            {

            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }
            CreateConnection();

            return _connection != null;
        }
    }
}
