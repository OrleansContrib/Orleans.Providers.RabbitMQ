using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Streams;
using RabbitMQ.Client;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQAdapterReceiver : IQueueAdapterReceiver
    {
        private RabbitMQStreamProviderConfig _config;
        private IConnection _connection;
        private IModel _model;

        public static IQueueAdapterReceiver Create(RabbitMQStreamProviderConfig config)
        {
            return new RabbitMQAdapterReceiver(config);
        }

        public RabbitMQAdapterReceiver(RabbitMQStreamProviderConfig config)
        {
            _config = config;
        }
        
        public async Task<IList<IBatchContainer>> GetQueueMessagesAsync(int maxCount)
        {
            return await Task.Run(async () =>
            {
                if (_connection == null)
                    await CreateConnection();
                var result = _model.BasicGet(_config.Queue, true);
                if (result == null)
                    return null;
                var container = new RabbitMQBatchContainer(result.Body);
                container.StreamNamespace = _config.Namespace;
                container.StreamGuid = Guid.Empty;
                Console.WriteLine($"Receiving for {container.StreamNamespace}:{container.StreamGuid}...");
                return new List<IBatchContainer> { container };
            });
        }

        private async Task CreateConnection()
        {
            var factory = new ConnectionFactory();
            factory.HostName = _config.HostName;
            factory.VirtualHost = _config.VirtualHost;
            factory.UserName = _config.Username;
            factory.Password = _config.Password;
            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();
            _model.ExchangeDeclare(_config.Exchange, ExchangeType.Direct, false, false, null);
            _model.QueueDeclare(_config.Queue, false, false, false, null);
            _model.QueueBind(_config.Queue, _config.Exchange, _config.RoutingKey, null);
        }

        public Task Initialize(TimeSpan timeout)
        {
            return TaskDone.Done;
        }

        public Task MessagesDeliveredAsync(IList<IBatchContainer> messages)
        {
            // TODO: Ack messages, if required.
            return TaskDone.Done;
        }

        public Task Shutdown(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }
    }
}