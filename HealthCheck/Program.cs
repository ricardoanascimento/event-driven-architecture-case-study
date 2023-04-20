using System.Text.Json;

namespace Level1OutageConsumer
{
    class Program
    {
        static int voltThreshold = int.TryParse(Environment.GetEnvironmentVariable("VOLT_THRESHOLD"), out int result) ? result : 100;
        static int minutesThreshold = int.TryParse(Environment.GetEnvironmentVariable("MINUTES_THRESHOLD"), out int result) ? result : 2;
        static int minutesFilter = (minutesThreshold + 1) * -1;

        static void Main(string[] args)
        {
            var dataHandler = new MongoDBDataHandler("mongodb://localhost:27017", "outages", "outageDocuments");
            var eventHandler = new RabbitMQEventHandler("localhost", "admin", "admin", "level1-outage");

            eventHandler.StartConsuming(async data =>
            {
                Console.WriteLine("Received message: {0}", JsonSerializer.Serialize(data));

                // Check if the voltage is below a certain threshold
                if (data.Volt < voltThreshold)
                {
                    // Persist every outage
                    var turbineId = data.TurbineId;
                    await dataHandler.InsertAsync(data);

                    //Verify if it is a recurrent outage
                    var lastReadings = await dataHandler.GetListWithinTimeFrameByTurbineIdAsync(turbineId, DateTime.UtcNow.AddMinutes(minutesFilter));
                    var isEveryThingAnOutage = !lastReadings.Any(e => e.Volt >= voltThreshold);

                    if (isEveryThingAnOutage)
                    {
                        var outageWithinTimeFrame = lastReadings.Any(doc => Math.Abs((data.TimeStamp - doc.TimeStamp).TotalMinutes) >= minutesThreshold);
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