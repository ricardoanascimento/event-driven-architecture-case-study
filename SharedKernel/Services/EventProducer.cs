using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SharedKernel.Services
{
    public class EventProducer<T> : RabbitMQConnectionHandler, IEventProducer<T>
    {
        protected readonly string _exchangeName;
        protected readonly string _routingKey;
        protected readonly IModel _channel;

        public EventProducer(string hostName, string userName, string password, string exchangeName, string routingKey)
        : base(hostName, userName, password)
        {
            _exchangeName = exchangeName;
            _routingKey = routingKey;
            _channel = CreateModel();
        }

        public void PublishEvent(T message)
        {
            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            _channel.BasicPublish(exchange: _exchangeName,
                      routingKey: _routingKey,
                      basicProperties: null,
                      body: messageBody);
        }
    }
}