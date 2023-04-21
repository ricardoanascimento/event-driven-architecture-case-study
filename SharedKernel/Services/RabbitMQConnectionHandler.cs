using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedKernel.Models;

namespace SharedKernel.Services
{
    public class RabbitMQConnectionHandler : IDisposable
    {
        protected readonly IConnection _connection;
        public RabbitMQConnectionHandler(string hostName, string userName, string password)
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };
            _connection = factory.CreateConnection();
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
    }
}