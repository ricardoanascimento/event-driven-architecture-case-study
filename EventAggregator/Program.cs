using System.Text.Json;
using SharedKernel.Models;
using SharedKernel.Services;

namespace EventAggregator
{
    class Program
    {
        static int minutesThreshold = int.TryParse(Environment.GetEnvironmentVariable("MINUTES_THRESHOLD"), out int result) ? result : 2;

        static void Main(string[] args)
        {
            var dataHandler = new EventAggregatorDataHandler("mongodb://localhost:27017", "aggregates", "outageDocuments");
            var eventConsumer = new EventConsumer<TurbineData>("localhost", "admin", "admin", "level2-outage");

            eventConsumer.StartConsuming(async data =>
            {
                Console.WriteLine("Received message: {0}", JsonSerializer.Serialize(data));

                // Persist every outage
                var turbineId = data.TurbineId;
                if (turbineId != null)
                {
                    var earliestDate = DateTime.UtcNow.AddMinutes(-minutesThreshold);
                    var latestAggregateDocument = await dataHandler.GetLatestByTurbineId(turbineId, earliestDate);

                    if (latestAggregateDocument != null)
                    {
                        latestAggregateDocument.Telemetries?.Add(data);
                        latestAggregateDocument.TotalVolts += data.Volt;
                        latestAggregateDocument.UpdatedAt = data.TimeStamp;
                        await dataHandler.ReplaceOneAsync(latestAggregateDocument);
                    }
                    else
                    {
                        var turbineDataAggregate = new TurbineDataAggregateEntity()
                        {
                            TurbineId = turbineId,
                            TotalVolts = data.Volt,
                            Telemetries = new List<TurbineData>() { data },
                            CreatedAt = data.TimeStamp,
                            UpdatedAt = data.TimeStamp
                        };
                        await dataHandler.InsertAsync(turbineDataAggregate);
                    }
                }
            });
        }
    }
}