using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

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
            channel.ExchangeDeclare(exchange: "turbine-topic-exchange", type: "topic", durable: true);

            // Generate random turbine IDs
            var turbineIds = new List<string>
            {
                Guid.NewGuid().ToString(),
                // Guid.NewGuid().ToString(),
                // Guid.NewGuid().ToString(),
            };

            // Simulate turbine data and publish it to RabbitMQ
            var random = new Random();
            while (true)
            {
                var data = new TurbineData
                {
                    TurbineId = turbineIds[random.Next(turbineIds.Count)],
                    // Volt = (float)random.NextDouble() * 1000,
                    Volt = 90,
                    Amp = (float)random.NextDouble() * 100,
                    RPM = random.Next(1000, 2000),
                    TimeStamp = DateTime.UtcNow
                };
                var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
                channel.BasicPublish(exchange: "turbine-exchange",
                                      routingKey: "turbine-exchange.telemetry",
                                      basicProperties: null,
                                      body: messageBody);
                Console.WriteLine("Sent message: {0}", JsonSerializer.Serialize(data));
                Task.Delay(1000).Wait();
            }
        }
    }
}