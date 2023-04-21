using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SharedKernel.Services
{
    public class EventConsumer<T> : RabbitMQConnectionHandler, IEventConsumer<T>
    {
        protected readonly string _queueName;

        public EventConsumer(string hostName, string userName, string password, string queueName)
        : base(hostName, userName, password)
        {
            _queueName = queueName;
        }

        public void StartConsuming(Action<T> onMessageReceived)
        {
            using (var channel = CreateModel())
            {
                channel.QueueDeclare(queue: _queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var data = JsonSerializer.Deserialize<T>(message);

                    // Invoke the message received delegate with the deserialized message
                    if (data != null)
                        onMessageReceived(data);
                };

                channel.BasicConsume(queue: _queueName,
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Started consuming messages from queue: {0}", _queueName);
                Console.ReadLine();
            }
        }
    }
}