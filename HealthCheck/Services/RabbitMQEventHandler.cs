using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
public class RabbitMQEventHandler : IEventHandler
{
    private readonly IConnection _connection;
    private readonly string _queueName;

    public RabbitMQEventHandler(string hostName, string userName, string password, string queueName)
    {
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password
        };
        _connection = factory.CreateConnection();
        _queueName = queueName;
    }

    public IModel CreateModel()
    {
        return _connection.CreateModel();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public IModel CreateChannel()
    {
        return _connection.CreateModel();
    }

    public void StartConsuming(Action<TurbineData> onMessageReceived)
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
                var data = JsonSerializer.Deserialize<TurbineData>(message);

                // Invoke the message received delegate with the deserialized message
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