using System.Text.Json;
using SharedKernel.Models;
using SharedKernel.Services;

class Program
{
    static void Main(string[] args)
    {
        var eventProducer = new EventProducer<TurbineData>("localhost", "admin", "admin", "turbine-exchange", "turbine-exchange.telemetry");

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
            var turbineData = new TurbineData
            {
                TurbineId = turbineIds[random.Next(turbineIds.Count)],
                // Volt = (float)random.NextDouble() * 1000,
                Volt = 90,
                Amp = (float)random.NextDouble() * 100,
                RPM = random.Next(1000, 2000),
                TimeStamp = DateTime.UtcNow
            };

            eventProducer.PublishEvent(turbineData);

            Console.WriteLine("Sent message: {0}", JsonSerializer.Serialize(turbineData));
            Task.Delay(1000).Wait();
        }
    }
}