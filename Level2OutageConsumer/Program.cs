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

            // Create a queue for level 2 outages
            var level2QueueName = "level2-outage";
            channel.QueueDeclare(queue: level2QueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Bind the level 2 queue to the exchange using the routing key
            channel.QueueBind(queue: level2QueueName,
                              exchange: "turbine-topic-exchange",
                              routingKey: "turbine-topic-exchange.telemetry");

            Console.WriteLine(" [*] Waiting for turbine data.");

            var level2Consumer = new EventingBasicConsumer(channel);
            level2Consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var data = JsonSerializer.Deserialize<TurbineData>(message);
                // Console.WriteLine(" [x] Received {0}", message);

                // Check if the voltage is below a certain threshold
                if (data.Volt < 50)
                {
                    Console.WriteLine("ALERT: Level 2 power outage detected for turbine {0}", data.TurbineId);
                }
            };
            channel.BasicConsume(queue: level2QueueName,
                                 autoAck: true,
                                 consumer: level2Consumer);

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
