using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "admin"
        };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "turbine-topic-exchange",
                                    type: "topic",
                                    durable: true);

            // Create a queue for level 1 outages
            var level1QueueName = "level1-outage";
            channel.QueueDeclare(queue: level1QueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Bind the level 1 queue to the exchange using the routing key
            channel.QueueBind(queue: level1QueueName,
                              exchange: "turbine-topic-exchange",
                              routingKey: "turbine-topic-exchange.telemetry");

            Console.WriteLine(" [*] Waiting for turbine data.");

            var level1Consumer = new EventingBasicConsumer(channel);
            level1Consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var data = JsonSerializer.Deserialize<TurbineData>(message);
                // Console.WriteLine(" [x] Received {0}", message);

                // Check if the voltage is below a certain threshold
                if (data.Volt < 100)
                {
                    Console.WriteLine("ALERT: Level 1 power outage detected for turbine {0}", data.TurbineId);
                }
            };
            channel.BasicConsume(queue: level1QueueName,
                                 autoAck: true,
                                 consumer: level1Consumer);

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
