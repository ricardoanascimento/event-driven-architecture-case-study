using System.Text.Json;

namespace Level1OutageConsumer
{
    class Program
    {
        const int VOLT_THRESHOLD = 100;
        const int MINUTES_THRESHOLD = 2;
        const int MINUTES_FILTER = (MINUTES_THRESHOLD + 1) * -1;

        static void Main(string[] args)
        {
            var dataHandler = new MongoDBDataHandler("mongodb://localhost:27017", "outages", "outageDocuments");
            var eventHandler = new RabbitMQEventHandler("localhost", "admin", "admin", "level1-outage");

            eventHandler.StartConsuming(async data =>
            {
                Console.WriteLine("Received message: {0}", JsonSerializer.Serialize(data));

                // Check if the voltage is below a certain threshold
                if (data.Volt < VOLT_THRESHOLD)
                {
                    // Persist every outage
                    var turbineId = data.TurbineId;
                    await dataHandler.InsertAsync(data);

                    //Verify if it is a recurrent outage
                    var lastReadings = await dataHandler.GetListWithinTimeFrameByTurbineIdAsync(turbineId, DateTime.UtcNow.AddMinutes(MINUTES_FILTER));
                    var isEveryThingAnOutage = !lastReadings.Any(e => e.Volt >= VOLT_THRESHOLD);

                    if (isEveryThingAnOutage)
                    {
                        var outageWithinTimeFrame = lastReadings.Any(doc => Math.Abs((data.TimeStamp - doc.TimeStamp).TotalMinutes) >= MINUTES_THRESHOLD);
                        if (outageWithinTimeFrame)
                        {
                            Console.WriteLine("ALERT: Level 1 power outage detected for turbine {0}", data.TurbineId);
                        }
                    }
                }
            });
        }
    }
}